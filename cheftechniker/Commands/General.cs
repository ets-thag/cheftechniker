using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace cheftechniker.Commands;

public class General
{
    [Command("ping")]
    public static async Task Ping(CommandContext ctx)
    {
        await ctx.RespondAsync(
            new DiscordInteractionResponseBuilder()
                .WithContent("Pong!\ud83c\udfd3")
        );
    }
}