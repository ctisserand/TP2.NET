using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Game> Games { get; set; }

        public DbSet<Game.Category> Categories { get; set; }
        public IEnumerable<object> GameUser { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurer la relation many-to-many
            modelBuilder.Entity<User>()
                .HasMany(u => u.PurchasedGames)
                .WithMany(g => g.PurchasedByUsers)  // Assumes you have a 'PurchasedByUsers' collection in Game
                .UsingEntity(j => j.ToTable("GameUser"));  // Nom de la table de jointure
        }
    }
}
