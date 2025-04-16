using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Modules
{
    internal static class Logger
    {
        public static async Task LogDeletedMessage(DiscordClient client, MessageDeletedEventArgs eventArgs)
        {
            if (eventArgs.Message.Author == null) return;

            var guild = eventArgs.Guild;
            var logChannelId = await Database.GetLogChannel(guild.Id);
            if (logChannelId == 0UL) return;

            var logChannel = guild.GetChannelAsync(logChannelId);
            
            var avatarUrl = eventArgs.Message.Author.AvatarUrl;

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{eventArgs.Message.Author.Username}",
                    avatarUrl, // This argument makes the username a clickable link to the avatar
                    avatarUrl)
                .WithTitle("🗑️ Message Deleted")
                .WithDescription(
                    $"**Message sent by** {eventArgs.Message.Author.Mention} **deleted in** {eventArgs.Channel.Mention}")
                .AddField("Message ID", eventArgs.Message.Id.ToString(), true)
                .AddField("Content", eventArgs.Message.Content)
                .AddField("Author ID", eventArgs.Message.Author.Id.ToString(), true)
                .WithColor(DiscordColor.Red)
                .WithTimestamp(DateTimeOffset.UtcNow);

            await client.SendMessageAsync(await logChannel, embed: embed.Build());
        }

        public static async Task LogUpdatedMessage(DiscordClient client, MessageUpdatedEventArgs eventArgs)
        {
            if (eventArgs.Message.Content == eventArgs.MessageBefore?.Content) return; // skip formatting edits

            var guild = eventArgs.Guild;
            var logChannelId = await Database.GetLogChannel(guild.Id);
            if (logChannelId == 0UL) return;
            var logChannel = guild.GetChannelAsync(logChannelId);
            if (eventArgs.Message.Author != null)
            {
                var avatarUrl = eventArgs.Message.Author.AvatarUrl;

                var embed = new DiscordEmbedBuilder()
                    .WithAuthor($"{eventArgs.Author.Username}",
                        avatarUrl, // This argument makes the username a clickable link to the avatar
                        avatarUrl)
                    .WithTitle("✏️ Message Edited")
                    .WithDescription(
                        $"**Message by** {eventArgs.Author.Mention} **edited in** {eventArgs.Channel.Mention}")
                    .AddField("Before", eventArgs.MessageBefore?.Content ?? "*No content*")
                    .AddField("After", eventArgs.Message.Content)
                    .AddField("Message ID", eventArgs.Message.Id.ToString(), true)
                    .WithColor(DiscordColor.Blue)
                    .WithTimestamp(DateTimeOffset.UtcNow);

                await client.SendMessageAsync(await logChannel, embed: embed.Build());
            }
        }
    }
}