namespace pfDataSource.ApiTests;

/// <summary>
/// Every test class that needs the host joins this collection, so a single <see cref="AppStartupFactory"/> is
/// built once and the classes run one after another. The host cannot be shared in parallel: its connection string
/// comes from a process-wide environment variable and Hangfire keeps global static configuration.
/// </summary>
[CollectionDefinition(Name)]
public sealed class AppCollection : ICollectionFixture<AppStartupFactory>
{
    public const string Name = "App host";
}
