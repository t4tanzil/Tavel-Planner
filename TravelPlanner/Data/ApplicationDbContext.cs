using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Models;

namespace TravelPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Attraction> Attractions { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships to avoid cascade delete issues
            modelBuilder.Entity<City>()
                .HasOne(c => c.Country)
                .WithMany(co => co.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attraction>()
                .HasOne(a => a.Country)
                .WithMany(c => c.Attractions)
                .HasForeignKey(a => a.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attraction>()
                .HasOne(a => a.City)
                .WithMany(c => c.Attractions)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Attraction)
                .WithMany(a => a.Hotels)
                .HasForeignKey(h => h.AttractionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.City)
                .WithMany(c => c.Hotels)
                .HasForeignKey(h => h.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}