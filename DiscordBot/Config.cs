using Microsoft.Extensions.Configuration;

namespace DiscordBot
{
    internal static class Config
    {
        private static IConfigurationRoot Settings { get; set; } = null!;

        public static void Init(string file = "config.json")
        {
            Settings = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(file, optional: false, reloadOnChange: true)
                .Build();
        }

        public static string Token =>
            Settings["Token"] ?? throw new MissingFieldException("Token is needed to start the bot");

        // ATM I just use a set log channel from my testing guild. But the log channel should be set on a per-guild basis
        public static ulong LogChannelId => ulong.Parse(Settings["LogChannelId"] ??
                                                        throw new MissingFieldException(
                                                            "Log channel id is needed (atm) to start the bot"));

        public static ulong DebugGuildId => ulong.Parse(Settings["DebugGuildId"] ??
                                                        throw new MissingFieldException(
                                                            "Debug guild id is needed (atm) to start the bot"));
    }
}