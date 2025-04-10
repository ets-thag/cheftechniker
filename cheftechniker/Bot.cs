using cheftechniker.Commands;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace cheftechniker;

public class Bot
{
    public async Task RunAsync()
    {
        var builder = DiscordClientBuilder.CreateDefault(Config.Token, DiscordIntents.All);

        builder.UseCommands((_, extension) =>
        {
            extension.AddCommands([typeof(Fun), typeof(General)]);
            var slashCommandProcessor = new SlashCommandProcessor();
            extension.AddProcessor(slashCommandProcessor);
        }, new CommandsConfiguration()
        {
            DebugGuildId = Config.DebugGuildId,
        });

        builder.ConfigureEventHandlers(
            b => b.HandleMessageDeleted(Logger.OnMessageDeleted)
                .HandleMessageUpdated(Logger.OnMessageUpdated));

        var client = builder.Build();

        DiscordActivity status = new("with C# and DSharpPlus", DiscordActivityType.Playing);

        await client.ConnectAsync(status, DiscordUserStatus.Online);

        Console.WriteLine("Bot is running...");
        await Task.Delay(-1);
    }
}