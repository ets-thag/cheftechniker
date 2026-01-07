namespace DiscordBot;

internal static class Program
{
    private static async Task Main()
    {
        Config.Init();
        await Bot.RunAsync();
    }
}