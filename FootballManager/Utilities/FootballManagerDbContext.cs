using Microsoft.EntityFrameworkCore;
using FootballManager.Models;

namespace FootballManager.Data
{
    public class FootballManagerDbContext : DbContext
    {

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }

            // Constructor for ASP.NET Core DI
        public FootballManagerDbContext(DbContextOptions<FootballManagerDbContext> options)
            : base(options)
        {
        }

        // Parameterless constructor for console app
        public FootballManagerDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=footballmanager.db");
        }
    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Players)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.TeamId)
                .IsRequired(false); // Allow nullable TeamId

            modelBuilder.Entity<Team>()
                .HasMany(t => t.StaffMembers)
                .WithOne(s => s.Team)
                .HasForeignKey(s => s.TeamId)
                .IsRequired(false); // Allow nullable TeamId

            modelBuilder.Entity<League>()
                .HasMany(l => l.Teams)
                .WithOne(t => t.League)
                .HasForeignKey(t => t.LeagueId)
                .IsRequired();
        }
    }
}