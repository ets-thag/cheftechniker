using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace DiscordBot.Commands;

public class Math
{
    [Command("latex")]
    [Description("Render LaTeX math expressions.")]
    public static async Task RenderLatex(CommandContext ctx,
        [Description("The LaTeX expression to render.")] string expression)
    {
        const string quickLatexUrl = "https://www.quicklatex.com/latex3.f";

        var values = new Dictionary<string, string>
        {
            { "formula", expression.Replace(" ", "") },
            { "fsize", "30px" },
            { "fcolor", "ffffff" }, // white text
            { "mode", "0" }, // display mode
            { "out", "1" }, // return image URL
            { "errors", "1" } // include error output if compile fails
        };

        using var httpClient = new HttpClient();
        using var content = new FormUrlEncodedContent(values);

        try
        {
            var response = await httpClient.PostAsync(quickLatexUrl, content);
            var body = await response.Content.ReadAsStringAsync();
            
            var lines = body.Split('\n');
            
            if (lines[0].Equals("0"))
            {
                var errorMessage = lines.Length > 1 ? string.Join("\n", lines.Skip(2)) : "Unknown LaTeX error.";
                await ctx.RespondAsync(new DiscordEmbedBuilder
                {
                    Title = "❌ LaTeX Render Error",
                    Description = $"```\n{errorMessage.Trim()}\n```",
                    Color = DiscordColor.Red
                });
                return;
            }

            var imageUrl = lines[1];

            var tempFile = Path.Combine(Path.GetTempPath(), $"latex_{Guid.NewGuid()}.png");

            var imageResponse = await httpClient.GetAsync(imageUrl.Split(" ")[0]);
            if (!imageResponse.IsSuccessStatusCode)
            {
                await ctx.RespondAsync("❌ Failed to fetch the rendered LaTeX image.");
                return;
            }

            await using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                await imageResponse.Content.CopyToAsync(fs);
            }

            await ctx.RespondAsync(new DiscordInteractionResponseBuilder()
                .AddFile("latex.png", new FileStream(tempFile, FileMode.Open, FileAccess.Read)));
        }
        catch (Exception ex)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder
            {
                Title = "⚠️ Unexpected Error",
                Description = $"Something went wrong:\n```\n{ex.Message}\n```",
                Color = DiscordColor.DarkRed
            });
        }
    }
}