using System.Diagnostics;

namespace DiscordBot
{
    public static class VersionInfo
    {
        public static string GetVersion()
        {
            return RunGit("rev-parse --short HEAD") ?? "unknown";
        }

        public static string GetChangelog(int count = 5)
        {
            return RunGit($"log -n {count} --pretty=format:\"• %s (%cr)\"") ?? "No changelog available.";
        }

        private static string? RunGit(string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppContext.BaseDirectory
                };

                using var process = Process.Start(psi);
                return process?.StandardOutput.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
    }   
}