using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace DiscordBot.Commands;

public class Fun
{
    private static readonly HttpClient Http = new();

    [Command("cat")]
    [Description("Get a random cat picture.")]
    public static async Task SendCatImage(CommandContext ctx)
    {
        const string imageUrl = "https://cataas.com/cat";

        // The picture has to be saved to a file because otherwise I would always get the same file
        var tmpFolder = Path.Combine(AppContext.BaseDirectory, "tmp");
        Directory.CreateDirectory(tmpFolder);
        var filePath = Path.Combine(tmpFolder, "cat.jpg");

        using (var response = await Http.GetAsync(imageUrl))
        {
            response.EnsureSuccessStatusCode();
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await response.Content.CopyToAsync(fileStream);
        }

        var builder = new DiscordEmbedBuilder()
            .WithTitle("Here's a cute cat!")
            .WithImageUrl($"attachment://cat.jpg")
            .WithColor(DiscordColor.Blurple);

        await ctx.RespondAsync(
            new DiscordInteractionResponseBuilder()
                .AddFile("cat.jpg", new FileStream(filePath, FileMode.Open, FileAccess.Read))
                .AddEmbed(builder)
        );
    }
}