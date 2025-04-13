// See https://aka.ms/new-console-template for more information

using Dapper;
using MySqlConnector;

namespace Database_Testing;

public class Guild
{
    public ulong GuildId { get; init; }
    public ulong LogchannelId { get; init; }
}

public abstract class Program
{
    private const string ConnectionString = "Server=localhost;Database=thagbot;User ID=cheftechniker;";

    public static async Task Main()
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        var guild = new Guild
        {
            GuildId = 123456789012345678,
            LogchannelId = 987654321098765432
        };

        try
        {
            const string insertQuery = "INSERT INTO guilds VALUES (@GuildId, @LogchannelId)";
            await connection.ExecuteAsync(insertQuery, guild);
            Console.WriteLine($"({guild.GuildId}, {guild.LogchannelId}) inserted successfully into guilds.");
        }
        catch (Exception)
        {
            Console.WriteLine("Key already exists in the database.");
        }
    }
}