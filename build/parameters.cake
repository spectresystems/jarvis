public sealed class Parameters
{
    public string GitHubUsername { get; set; }
    public string GitHubPassword { get; set; }

    public static Parameters Initialize(ICakeContext context)
    {
        return new Parameters
        {
            GitHubUsername = context.EnvironmentVariable("JARVIS_GITHUB_USERNAME"),
            GitHubPassword = context.EnvironmentVariable("JARVIS_GITHUB_PASSWORD")
        };
    }
}