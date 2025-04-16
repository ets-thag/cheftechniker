using System.ComponentModel;
using DiscordBot.Modules;
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

		string[] medals = ["🥇", "🥈", "🥉"];

       var embed = new DiscordEmbedBuilder
    {
        Title = $"🎙️ VC Leaderboard — {ctx.Guild.Name}",
        Color = DiscordColor.Blurple,
        Timestamp = DateTimeOffset.Now,
        Footer = new DiscordEmbedBuilder.EmbedFooter
        {
            Text = $"Requested by {ctx.User.Username}",
            IconUrl = ctx.User.AvatarUrl
        }
    };

    var rank = 1;
    var voiceActivities = topUsers as Database.VoiceActivity[] ?? topUsers.ToArray();
    foreach (var user in voiceActivities)
    {
        var member = await ctx.Guild.GetMemberAsync(user.UserId);
        var medal = rank <= 3 ? medals[rank - 1] : $"#{rank}";
        var displayName = member.DisplayName;
        var minutes = $"{user.MinutesInVc:N0} mins";

        embed.AddField($"{medal} {displayName}", minutes, inline: false);
        rank++;
    }

    if (voiceActivities.Length == 0)
    {
        embed.Description = "No voice activity tracked yet.";
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