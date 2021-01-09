using IdentityApp.Infrastructure.Database.Entities;
using IdentityApp.Infrastructure.Database.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Client = IdentityApp.Infrastructure.Database.Entities.Client;

namespace IdentityApp.Infrastructure.Database
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        static ApplicationDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<TransactionType>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<TransactionStatus>();
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresEnum<TransactionType>();
            builder.HasPostgresEnum<TransactionStatus>();

            builder.Entity<Client>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        public static readonly Microsoft.Extensions.Logging.LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
        }
    }
}
