using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace cheftechniker
{
    internal static class Logger
    {
        public static async Task OnMessageDeleted(DiscordClient client, MessageDeletedEventArgs eventArgs)
        {
            
            if (eventArgs.Message.Author == null) return;
            
            var logChannel = eventArgs.Guild.GetChannelAsync(Config.LogChannelId);
            
            Console.WriteLine(eventArgs.Message.Content);

            var avatarUrl = eventArgs.Message.Author.AvatarUrl ?? eventArgs.Message.Author.DefaultAvatarUrl;


            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{eventArgs.Message.Author.Username}",
                    avatarUrl, // This argument makes the username a clickable link to the avatar
                    avatarUrl)
                .WithTitle("🗑️ Message Deleted")
                .WithDescription(
                    $"**Message sent by** {eventArgs.Message.Author.Mention} **deleted in** {eventArgs.Channel.Mention}")
                .AddField("Message ID", eventArgs.Message.Id.ToString(), true)

                .AddField("Author ID", eventArgs.Message.Author.Id.ToString(), true)
                .WithColor(DiscordColor.Red)
                .WithTimestamp(DateTimeOffset.UtcNow);

            await client.SendMessageAsync(await logChannel, embed: embed.Build());
        }

        public static async Task OnMessageUpdated(DiscordClient client, MessageUpdatedEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Message.Content);
            if (eventArgs.Message.Content == eventArgs.MessageBefore?.Content) return; // skip formatting edits

            var logChannel = eventArgs.Guild.GetChannelAsync(Config.LogChannelId);

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