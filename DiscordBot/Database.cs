using Dapper;
using MySqlConnector;

namespace DiscordBot;

public abstract class Database
{
    private const string ConnectionString = "Server=localhost;Database=thagbot;User ID=cheftechniker;";

    // Guilds Stuff //

    public static async Task<bool> InsertGuild(ulong guildId, ulong logChannelId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        var guild = new Guild
        {
            GuildId = guildId,
            LogchannelId = logChannelId
        };

        try
        {
            const string insertQuery = "INSERT INTO guilds VALUES (@GuildId, @LogchannelId)";
            await connection.ExecuteAsync(insertQuery, guild);
            Console.WriteLine($"({guild.GuildId}, {guild.LogchannelId}) inserted successfully into guilds.");
            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Key already exists in the database.");
            return false;
        }
    }

    public static async Task<bool> DeleteGuild(ulong guildId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string deleteQuery = "DELETE FROM guilds WHERE guild_id = @GuildId";
            await connection.ExecuteAsync(deleteQuery, new { GuildId = guildId });
            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Error deleting the guild from the database.");
            return false;
        }
    }

    public static async Task<bool> UpdateLogChannel(ulong guildId, ulong logChannelId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string updateQuery = "UPDATE guilds SET logchannel_id = @LogchannelId WHERE guild_id = @GuildId";
            await connection.ExecuteAsync(updateQuery, new { GuildId = guildId, LogchannelId = logChannelId });
            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Error updating the log channel in the database.");
            return false;
        }
    }

    public static async Task<ulong> GetLogChannel(ulong guildId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        var logChannelId = 0UL;
        await connection.OpenAsync();

        try
        {
            const string selectQuery = "SELECT logchannel_id FROM guilds WHERE guild_id = @GuildId";
            logChannelId = await connection.QuerySingleAsync<ulong>(selectQuery, new { GuildId = guildId });
        }
        catch (Exception)
        {
            Console.WriteLine("Error retrieving the log channel from the database.");
        }

        return logChannelId;
    }

    public static async Task<bool> GuildExists(ulong guildId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string selectQuery = "SELECT COUNT(*) FROM guilds WHERE guild_id = @GuildId";
            var count = await connection.ExecuteScalarAsync<int>(selectQuery, new { GuildId = guildId });
            return count > 0;
        }
        catch (Exception)
        {
            Console.WriteLine("Error checking if the guild exists in the database.");
            return false;
        }
    }

    public static async Task<bool> LogChannelSet(ulong guildId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string selectQuery = "SELECT logchannel_id FROM guilds WHERE guild_id = @GuildId";
            var logChannelId = await connection.ExecuteScalarAsync<int>(selectQuery, new { GuildId = guildId });
            return logChannelId != 0;
        }
        catch (Exception)
        {
            Console.WriteLine("Error checking if the log channel is set in the database.");
            return false;
        }
    }

    private class Guild
    {
        public ulong GuildId { get; init; }
        public ulong LogchannelId { get; init; }
    }

    // Leaderboard Stuff //

    public static async Task<bool> UserInGuildExists(ulong guildId, ulong userId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string selectQuery =
                "SELECT COUNT(*) FROM voice_activity WHERE guild_id = @GuildId AND user_id = @UserId";
            var count = await connection.ExecuteScalarAsync<int>(selectQuery,
                new { GuildId = guildId, UserId = userId });
            return count > 0;
        }
        catch (Exception)
        {
            Console.WriteLine("Error checking if the user exists in the database.");
            return false;
        }
    }

    public static async Task<bool> InsertUser(ulong guildId, ulong userId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        var voiceActivity = new VoiceActivity
        {
            GuildId = guildId,
            UserId = userId,
            MinutesInVc = 0
        };

        try
        {
            const string insertQuery =
                "INSERT INTO voice_activity (guild_id, user_id, minutes_in_vc) VALUES (@GuildId, @UserId, @MinutesInVc)";
            await connection.ExecuteAsync(insertQuery, voiceActivity);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error inserting the user into the database.");
            Console.WriteLine(e);
            return false;
        }
    }

    public static async Task<bool> IncrementMinutesInVc(ulong guildId, ulong userId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string updateQuery =
                "UPDATE voice_activity SET minutes_in_vc = minutes_in_vc + 1 WHERE guild_id = @GuildId AND user_id = @UserId";
            await connection.ExecuteAsync(updateQuery, new { GuildId = guildId, UserId = userId });
            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Error incrementing the minute in VC in the database.");
            return false;
        }
    }

    public static async Task<int> GetMinutesInVc(ulong guildId, ulong userId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        try
        {
            const string selectQuery =
                "SELECT minutes_in_vc FROM voice_activity WHERE guild_id = @GuildId AND user_id = @UserId";
            var minutesInVc =
                await connection.ExecuteScalarAsync<int>(selectQuery, new { GuildId = guildId, UserId = userId });
            return minutesInVc;
        }
        catch (Exception)
        {
            Console.WriteLine("Error retrieving the minutes in VC from the database.");
            return 0;
        }
    }

    // Get the top 10 users with the most minutes in VC
    public static async Task<IEnumerable<VoiceActivity>> GetTopVcUsers(ulong guildId)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        Console.WriteLine($"GuildId: {guildId}");
        try
        {
            const string selectQuery =
                "SELECT user_id AS UserId, guild_id as GuildID, minutes_in_vc AS MinutesInVc FROM voice_activity WHERE guild_id = @GuildId ORDER BY minutes_in_vc DESC LIMIT 10";
            var topVcUsers = await connection.QueryAsync<VoiceActivity>(selectQuery, new { GuildId = guildId });

            var voiceActivities = topVcUsers as VoiceActivity[] ?? topVcUsers.ToArray();
            foreach (var user in voiceActivities)
            {
                Console.WriteLine($"UserId: {user.UserId}, GuildId: {user.GuildId}, MinutesInVc: {user.MinutesInVc}");
            }

            return voiceActivities;
        }
        catch (Exception)
        {
            Console.WriteLine("Error retrieving the top VC users from the database.");
            return [];
        }
    }

    public class VoiceActivity
    {
        public ulong GuildId { get; init; }
        public ulong UserId { get; init; }
        public int MinutesInVc { get; init; }
    }
}