using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBot.Modules;

public abstract class EventHandlers
{
    public static async Task OnMessageDeleted(DiscordClient client, MessageDeletedEventArgs eventArgs)
    {
        await Logger.LogDeletedMessage(client, eventArgs);
    }

    public static async Task OnMessageUpdated(DiscordClient client, MessageUpdatedEventArgs eventArgs)
    {
        await Logger.LogUpdatedMessage(client, eventArgs);
    }

    public static async Task OnGuildDownloadCompleted(DiscordClient client, GuildDownloadCompletedEventArgs eventArgs)
    {
        foreach (var guildId in eventArgs.Guilds.Keys)
        {
            if (await Database.GuildExists(guildId)) continue;
            await Database.InsertGuild(guildId, 0UL);
            Console.WriteLine($"Guild {guildId} inserted into the database.");
        }
    }
}