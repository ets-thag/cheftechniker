using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot.Modules;

public abstract class BackgroundTasks
{
    public static async Task TrackVcActivity(DiscordClient client)
    {
        while (true)
        {
            var guilds = client.Guilds.Values;
            foreach (var guild in guilds)
            {
                var channels = guild.Channels.Values;
                foreach (var channel in channels)
                {
                    if (channel.Type != DiscordChannelType.Voice) continue;
                    var voiceState = channel.Users;
                    foreach (var user in voiceState)
                    {
                        if (!await Database.UserInGuildExists(guild.Id, user.Id))
                        {
                            await Database.InsertUser(guild.Id, user.Id);
                        }

                        await Database.IncrementMinutesInVc(guild.Id, user.Id);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
        // ReSharper disable once FunctionNeverReturns
    }
}