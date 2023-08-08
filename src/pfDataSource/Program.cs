using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pfDataSource.Areas.Identity;
using pfDataSource.Db;
using Hangfire;
using Hangfire.SQLite;
using pfDataSource.Services;
using pfDataSource;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using MoA.Platform.Onboarding.Common;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var logger = Log.Logger = ConsoleLoggerFactory.CreateConsoleLogger();

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .AddCommandLine(args)
    .AddEnvironmentVariables();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString))
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHangfire(x => x.UseSQLiteStorage(connectionString, new SQLiteStorageOptions
{
    SchemaName = "hangfire"
}));

builder.Services.AddHangfireServer();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddScoped<IDirectoryWatcherService, DirectoryWatcherService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IEncryptionProvider, EncryptionProvider>();
builder.Services.AddScoped<IDataSourceConfigurationService, DataSourceConfigurationService>();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton<IAwsCredentialsService, AwsCredentialsService>();
builder.Services.AddTransient(async p => await p.GetService<IAwsCredentialsService>().GetCredentialsAsync());
builder.Services.AddAWSService<IAmazonS3>(ServiceLifetime.Transient);
builder.Services.AddScoped<IPushToAwsService, PushToAwsService>();
builder.Services.AddSingleton<Serilog.ILogger>(logger);


var app = builder.Build();
GlobalConfiguration.Configuration.UseActivator(new DIJobActivator(app.Services));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapHangfireDashboard();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}

app.Run();

