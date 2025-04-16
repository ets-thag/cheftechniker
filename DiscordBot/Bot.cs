using DiscordBot.Commands;
using DiscordBot.Modules;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace DiscordBot;

public class Bot
{
    public static async Task RunAsync()
    {
        var builder = DiscordClientBuilder.CreateDefault(Config.Token, DiscordIntents.All);

        builder.UseCommands((_, extension) =>
        {
            extension.AddCommands([typeof(Fun), typeof(General), typeof(Logging), typeof(Leaderboard)]);
            var slashCommandProcessor = new SlashCommandProcessor();
            extension.AddProcessor(slashCommandProcessor);
        }, new CommandsConfiguration()
        {
            DebugGuildId = Config.DebugGuildId // Set to 0 to disable debug guild
        });

        builder.ConfigureEventHandlers(
            b => b.HandleMessageDeleted(EventHandlers.OnMessageDeleted)
                .HandleMessageUpdated(EventHandlers.OnMessageUpdated)
                .HandleGuildDownloadCompleted(EventHandlers.OnGuildDownloadCompleted)
        );

        var client = builder.Build();

        DiscordActivity status = new("with C# and DSharpPlus", DiscordActivityType.Playing);

        await client.ConnectAsync(status, DiscordUserStatus.Online);

        Console.WriteLine("Bot is starting...");
        await Task.Run(async () => await BackgroundTasks.TrackVcActivity(client));
        await Task.Delay(-1);
    }
}