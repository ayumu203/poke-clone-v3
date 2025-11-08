using Microsoft.EntityFrameworkCore;
using server.Models.Core;

namespace server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    // Core models
    public DbSet<Player> Players { get; set; }
    public DbSet<Pokemon> Pokemons { get; set; }
    public DbSet<PokemonSpecies> PokemonSpecies { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<LearnedMove> LearnedMoves { get; set; }
    // Battle models
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Player
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId);
            entity.HasMany(e => e.Pokemons)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PokemonSpecies
        modelBuilder.Entity<PokemonSpecies>(entity =>
        {
            entity.HasKey(e => e.PokemonSpeciesId);
            entity.HasMany(e => e.Pokemons)
                .WithOne(e => e.Species)
                .HasForeignKey(e => e.PokemonSpeciesId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Move
        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasKey(e => e.MoveId);
            entity.OwnsOne(e => e.Rank); // Rankを複合型として扱う
        });

        // Pokemon
        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.HasKey(e => e.PokemonId);
            entity.OwnsOne(e => e.Rank); // Rankを複合型として扱う

            entity.HasMany(e => e.LearnedMoves)
                .WithOne(e => e.Pokemon)
                .HasForeignKey(e => e.PokemonId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        // LearnedMove
        modelBuilder.Entity<LearnedMove>(entity =>
        {
            entity.HasKey(e => e.LearnedMoveId);
            
            entity.HasOne(e => e.Move)
                .WithMany()
                .HasForeignKey(e => e.MoveId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}