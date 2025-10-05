using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using server.Models.Core;
using server.Models.Game;

namespace server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Player> Players { get; set; }
        public DbSet<PokemonSpecies> PokemonSpecies { get; set; }
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<PlayerParty> PlayerParties { get; set; }
        public DbSet<PokemonMove> PokemonMoves { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PokemonMove>()
                .HasKey(pm => new { pm.PokemonSpeciesId, pm.MoveId });

            modelBuilder.Entity<PlayerParty>()
                .HasKey(pp => new { pp.PlayerId, pp.PokemonId });

            modelBuilder.Entity<PlayerParty>()
                .HasOne(pp => pp.Player)
                .WithMany(p => p.PlayerParties)
                .HasForeignKey(pp => pp.PlayerId);

            modelBuilder.Entity<PlayerParty>()
                .HasOne(pp => pp.Pokemon)
                .WithMany()
                .HasForeignKey(pp => pp.PokemonId)
                .OnDelete(DeleteBehavior.Restrict);
            // 技の多重度
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Move1)
                .WithMany()
                .HasForeignKey(p => p.Move1Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Move2)
                .WithMany()
                .HasForeignKey(p => p.Move2Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Move3)
                .WithMany()
                .HasForeignKey(p => p.Move3Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Move4)
                .WithMany()
                .HasForeignKey(p => p.Move4Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}