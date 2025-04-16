using System.ComponentModel;
using DiscordBot.Modules;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;

namespace DiscordBot.Commands;

public class Logging
{
    [Command("SetLogchannel")]
    [Description("Set the log channel for the bot.")]
    [RequirePermissions(DiscordPermission.Administrator)]
    public static async Task SetLogChannel(CommandContext ctx, DiscordChannel channel)
    {
        if (channel.Type != DiscordChannelType.Text)
        {
            await ctx.RespondAsync("The specified channel is not a text channel.");
            return;
        }

        await Database.UpdateLogChannel(ctx.Guild!.Id, channel.Id);
        await ctx.RespondAsync($"Log channel set to {channel.Mention}.");
    }
}