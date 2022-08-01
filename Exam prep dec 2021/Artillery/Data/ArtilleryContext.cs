namespace Artillery.Data
{
    using Artillery.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ArtilleryContext : DbContext
    {
        public ArtilleryContext() { }

        public ArtilleryContext(DbContextOptions options)
            : base(options) { }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Gun> Guns { get; set; }
        public DbSet<Shell> Shells { get; set; }
        public DbSet<CountryGun> CountriesGuns { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manufacturer>()
                .HasIndex(u => u.ManufacturerName)
                .IsUnique();

            modelBuilder.Entity<CountryGun>()
            .HasKey(c => new { c.CountryId, c.GunId });

            modelBuilder.Entity<CountryGun>()
                  .HasOne(c => c.Country)
                  .WithMany(x => x.CountriesGuns)
                  .HasForeignKey(x => x.CountryId);

            modelBuilder.Entity<CountryGun>()
               .HasOne(c => c.Gun)
               .WithMany(x => x.CountriesGuns)
               .HasForeignKey(x => x.GunId);

            modelBuilder.Entity<Gun>()
                .HasOne(x => x.Manufacturer)
                .WithMany(x => x.Guns)
                .HasForeignKey(x => x.ManufacturerId);

            modelBuilder.Entity<Gun>()
            .HasOne(x => x.Shell)
            .WithMany(x => x.Guns)
            .HasForeignKey(x => x.ShellId);
        }

    }
}
