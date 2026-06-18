namespace FirinApi.Configuration;

public class SeedSettings
{
    public const string SectionName = "Seed";

    public string AdminUsername { get; set; } = "admin";
    public string AdminPassword { get; set; } = string.Empty;
}
