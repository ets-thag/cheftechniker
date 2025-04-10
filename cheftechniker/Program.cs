namespace cheftechniker;

internal static class Program
{
    private static async Task Main()
    {
        Config.Init();
        var bot = new Bot();
        await bot.RunAsync();
    }
}