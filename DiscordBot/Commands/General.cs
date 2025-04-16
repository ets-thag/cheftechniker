using System.ComponentModel;
using DiscordBot.Modules;
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
        var ping = ctx.Client.GetConnectionLatency(ctx.Guild!.Id);
        
        await ctx.RespondAsync(
            new DiscordInteractionResponseBuilder()
                .WithContent($"Pong with {ping.Milliseconds}ms!\ud83c\udfd3")
        );
    }

    // Admin only //
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
    
    [Command("restart")]
    [Description("Restart the bot.")]
    [RequirePermissions(DiscordPermission.Administrator)]
    public static async Task Restart(CommandContext ctx)
    {
        await ctx.RespondAsync(
            new DiscordInteractionResponseBuilder()
                .WithContent("Restarting bot...\ud83d\ude80")
        );

        DiscordActivity status = new("Restarting!", DiscordActivityType.Playing);
        await ctx.Client.UpdateStatusAsync(status, DiscordUserStatus.DoNotDisturb);

        // This works because the systemd service will restart the bot on failure
        _ = Task.Delay(2000).ContinueWith(_ => Environment.Exit(-1));
    }
    
    [Command("version")]
    [Description("Get the current version of the bot.")]
    [RequirePermissions(DiscordPermission.Administrator)]
    public static async Task ShowVersionCommand(CommandContext ctx)
    {
        var version = VersionInfo.GetVersion();
        var changelog = VersionInfo.GetChangelog();

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"📦 Bot Version: `{version}`")
            .WithDescription("📝 Recent Changes:\n" + $"```md\n{changelog}\n```")
            .WithColor(DiscordColor.Green)
            .WithTimestamp(DateTimeOffset.UtcNow);

        await ctx.RespondAsync(embed: embed.Build());
    }
    
    [Command("update")]
    [Description("Update the bot to the latest version.")]
    [RequirePermissions(DiscordPermission.Administrator)]
    public static async Task UpdateCommand(CommandContext ctx)
    {
        
        DiscordActivity status = new("Updating!", DiscordActivityType.Playing);
        await ctx.Client.UpdateStatusAsync(status, DiscordUserStatus.DoNotDisturb);

        await ctx.RespondAsync(
            new DiscordInteractionResponseBuilder()
                .WithContent("Updating bot...\ud83d\ude80")
            );
        
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "./update.sh",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        Console.WriteLine($"Output: {output}");
        Console.WriteLine($"Error: {error}");
        await process.WaitForExitAsync();
    }
}