using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pfDataSource.Db.Models;

namespace pfDataSource.Db;

public class ApplicationDbContext : IdentityDbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SourceConfiguration> SourceConfigurations { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<CryptoKeys> CryptoKeys { get; set; }
}

