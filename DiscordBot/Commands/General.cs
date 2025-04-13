using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;

namespace DiscordBot.Commands;

public class General
{
    [Command("ping")]
    [Description("Ping the bot.")]
    public static async Task Ping(CommandContext ctx)
    {
        await ctx.RespondAsync(
            new DiscordInteractionResponseBuilder()
                .WithContent("Pong!\ud83c\udfd3")
        );
    }

    [Command("botinfo")]
    [Description("Get information about the bot.")]
    [RequirePermissions(DiscordPermission.Administrator)]
    public static async Task BotInfo(CommandContext ctx)
    {
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Guild specific information")
            .WithColor(DiscordColor.Purple)
            .WithTimestamp(DateTimeOffset.UtcNow);

        var logChannelId = await Database.GetLogChannel(ctx.Guild!.Id);
        if (logChannelId != 0)
        {
            var logChannel = await ctx.Guild.GetChannelAsync(logChannelId);
            embed.AddField("Log Channel", logChannel.Mention, true);
        }
        else
        {
            embed.AddField("Log Channel", "Not set", true);
        }

        await ctx.RespondAsync(embed: embed.Build());
    }
}