using DiscordBot.Commands;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
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
            extension.AddProcessor(new TextCommandProcessor());
        }, new CommandsConfiguration()
        {
            DebugGuildId = Config.DebugGuildId,
        });

        builder.ConfigureEventHandlers(
            b => b.HandleMessageDeleted(Logger.OnMessageDeleted)
                .HandleMessageUpdated(Logger.OnMessageUpdated)
                .HandleGuildDownloadCompleted(async (_, e) =>
                {
                    foreach (var guildId in e.Guilds.Keys)
                    {
                        if (await Database.GuildExists(guildId)) continue;
                        await Database.InsertGuild(guildId, 0UL);
                        Console.WriteLine($"Guild {guildId} inserted into the database.");
                    }
                })
        );

        var client = builder.Build();

        DiscordActivity status = new("with C# and DSharpPlus", DiscordActivityType.Playing);

        await client.ConnectAsync(status, DiscordUserStatus.Online);

        Console.WriteLine("Bot is running...");
        await Task.Run(async () => await BackgroundTasks.TrackVcActivity(client));
        await Task.Delay(-1);
    }
}