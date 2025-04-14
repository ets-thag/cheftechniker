using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace DiscordBot.Commands;

public class Leaderboard
{
    [Command("vcleaderboard")]
    [Description("Get the leaderboard for voice channel activity.")]
    public static async Task Vcleaderboard(CommandContext ctx)
    {
        var topUsers = await Database.GetTopVcUsers(ctx.Guild!.Id);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Voice Channel Leaderboard")
            .WithColor(DiscordColor.Purple)
            .WithTimestamp(DateTimeOffset.UtcNow);

        foreach (var user in topUsers)
        {
            Console.WriteLine($"UserId: {user.UserId}, GuildId: {user.GuildId}, MinutesInVc: {user.MinutesInVc}");
            if (user.UserId == 0) continue;
            var member = await ctx.Guild.GetMemberAsync(user.UserId);
            embed.AddField($"{member.DisplayName}", $"{user.MinutesInVc} minutes", true);
        }

        await ctx.RespondAsync(embed: embed.Build());
    }

    [Command("myvc")]
    [Description("Get your voice channel activity.")]
    public static async Task MyVc(CommandContext ctx)
    {
        var minutesInVc = await Database.GetMinutesInVc(ctx.Guild!.Id, ctx.User.Id);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Your Voice Channel Activity")
            .WithColor(DiscordColor.Purple)
            .WithTimestamp(DateTimeOffset.UtcNow)
            .AddField("Minutes in VC", $"{minutesInVc} minutes", true);

        await ctx.RespondAsync(embed: embed.Build());
    }
}