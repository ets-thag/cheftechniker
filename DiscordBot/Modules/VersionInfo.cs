using System.Diagnostics;

namespace DiscordBot.Modules
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
        
        public static string GetBranch()
        {
            return RunGit("rev-parse --abbrev-ref HEAD") ?? "unknown";
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