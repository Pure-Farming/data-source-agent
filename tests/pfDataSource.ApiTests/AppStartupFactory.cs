using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace pfDataSource.ApiTests;

/// <summary>
/// Boots the real Blazor Server host (the real Program: Identity, EF Core, Hangfire, Blazor, AWS wiring) in-memory.
/// Only the external edges are replaced: the SQLite file lives in a temp folder, and the AWS SDK gets dummy
/// region/credentials so nothing reaches for real credentials or the EC2 metadata service.
/// </summary>
/// <remarks>
/// <c>Program</c> is an internal top-level class, so the factory is anchored on a public type in the same assembly;
/// the type is only used to locate the assembly and its entry point.
/// Used as an xUnit class fixture: the host is built once and shared by every test in the class.
/// </remarks>
public sealed class AppStartupFactory : WebApplicationFactory<DIJobActivator>
{
    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"pfDataSource-smoke-{Guid.NewGuid():N}.db");

    static AppStartupFactory()
    {
        SetIfMissing("AWS_REGION", "us-east-1");
        SetIfMissing("AWS_ACCESS_KEY_ID", "smoke-test");
        SetIfMissing("AWS_SECRET_ACCESS_KEY", "smoke-test");
        SetIfMissing("AWS_EC2_METADATA_DISABLED", "true");
    }

    public AppStartupFactory()
    {
        // Program reads the connection string while it is still building the host, before the factory's
        // configuration callbacks apply, so it must be supplied through the environment.
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            $"DataSource={_databasePath};Cache=Shared");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // TestServer requests have no remote IP, which Hangfire's local-only dashboard filter dereferences.
        // Real servers always have one, so give every request a loopback address.
        builder.ConfigureTestServices(services => services.AddTransient<IStartupFilter, LoopbackRemoteAddressFilter>());
    }

    private sealed class LoopbackRemoteAddressFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
            app =>
            {
                app.Use((context, nextMiddleware) =>
                {
                    context.Connection.RemoteIpAddress ??= IPAddress.Loopback;
                    return nextMiddleware(context);
                });
                next(app);
            };
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;

        SqliteConnection.ClearAllPools();
        foreach (var path in new[] { _databasePath, _databasePath + "-shm", _databasePath + "-wal" })
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
                // Best effort: a temp file left behind is harmless.
            }
        }
    }

    private static void SetIfMissing(string name, string value)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
            Environment.SetEnvironmentVariable(name, value);
    }
}
