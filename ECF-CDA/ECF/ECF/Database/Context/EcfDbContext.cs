using ECF.Models;
using Microsoft.EntityFrameworkCore;

namespace ECF.Database.Context
{
    public class EcfDbContext : DbContext
    {
        public EcfDbContext(DbContextOptions<EcfDbContext> options)
        : base(options)
        {

        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Breed> Breeds { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .HasKey(animal => animal.AnimalId);

            modelBuilder.Entity<Animal>()
                .Property(animal => animal.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Animal>()
                .Property(animal => animal.Description)
                .HasMaxLength(2000)
                .IsRequired();

            modelBuilder.Entity<Animal>()
                .HasOne(animal => animal.Breed)
                .WithMany(breed => breed.Animals)
                .HasForeignKey(animal => animal.BreedId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------------------------------

            modelBuilder.Entity<Breed>()
                .HasKey(breed => breed.BreedId);

            modelBuilder.Entity<Breed>()
                .Property(breed => breed.BreedName)
                .IsRequired();

            modelBuilder.Entity<Breed>()
                .Property(breed => breed.Description)
                .HasMaxLength(2000)
                .IsRequired();
        }
    }
}
