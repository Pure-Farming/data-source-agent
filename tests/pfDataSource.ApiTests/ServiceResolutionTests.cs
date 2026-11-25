using Amazon.S3;
using FluentAssertions;
using Hangfire;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using pfDataSource.Db;
using pfDataSource.Services;
using pfDataSource.Services.Jobs;

namespace pfDataSource.ApiTests;

[Collection(AppCollection.Name)]
public class ServiceResolutionTests
{
    private readonly AppStartupFactory factory;

    public ServiceResolutionTests(AppStartupFactory factory) => this.factory = factory;

    public static TheoryData<Type> RegisteredServices => new()
    {
        typeof(IDirectoryWatcherService),
        typeof(IConfigurationService),
        typeof(IEncryptionProvider),
        typeof(IDataSourceConfigurationService),
        typeof(IPushToAwsService),
        typeof(IAwsCredentialsService),
        typeof(Serilog.ILogger),
        typeof(IAmazonS3),
        typeof(ApplicationDbContext),
        typeof(UserManager<IdentityUser>),
        typeof(SignInManager<IdentityUser>),
        typeof(AuthenticationStateProvider)
    };

    [Theory]
    [MemberData(nameof(RegisteredServices))]
    public void Registered_service_resolves(Type serviceType)
    {
        using var scope = factory.Services.CreateScope();

        var act = () => scope.ServiceProvider.GetRequiredService(serviceType);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(DbCheckJob))]
    [InlineData(typeof(FileCheckJob))]
    public void Job_dependencies_are_all_registered(Type jobType)
    {
        // The jobs themselves are not registered in the container, so check that everything they
        // depend on is by building them the way the container would.
        using var scope = factory.Services.CreateScope();

        var act = () => ActivatorUtilities.CreateInstance(scope.ServiceProvider, jobType);

        act.Should().NotThrow().Which.Should().BeAssignableTo<Job>();
    }

    [Fact]
    public void Job_activator_serves_registered_services_from_the_container()
    {
        var activator = new DIJobActivator(factory.Services);

        activator.ActivateJob(typeof(Serilog.ILogger)).Should().NotBeNull();
    }
}
