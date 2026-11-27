using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using pfDataSource.Db;

namespace pfDataSource.ApiTests;

[Collection(AppCollection.Name)]
public class HostSmokeTests
{
    private readonly AppStartupFactory factory;
    private readonly HttpClient _client;

    public HostSmokeTests(AppStartupFactory factory)
    {
        this.factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Host_boots_and_serves_the_home_page()
    {
        var response = await _client.GetAsync("/");

        // The page may render the app or redirect an anonymous user to login; it must never fail.
        ((int)response.StatusCode).Should().BeLessThan(500);
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Identity_login_page_is_served()
    {
        var response = await _client.GetAsync("/Identity/Account/Login");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Log in");
    }

    [Fact]
    public async Task Startup_applies_every_database_migration()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        (await db.Database.GetAppliedMigrationsAsync()).Should().NotBeEmpty();
        (await db.Database.GetPendingMigrationsAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task Hangfire_dashboard_is_mapped()
    {
        var response = await _client.GetAsync("/hangfire");

        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        ((int)response.StatusCode).Should().BeLessThan(500);
    }

    [Fact]
    public async Task Blazor_hub_negotiates()
    {
        var response = await _client.PostAsync("/_blazor/negotiate?negotiateVersion=1", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("connectionId");
    }
}
