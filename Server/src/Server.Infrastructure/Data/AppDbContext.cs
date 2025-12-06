using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Player> Players { get; set; }
    public DbSet<PokemonSpecies> PokemonSpecies { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Pokemon> Pokemons { get; set; }
    public DbSet<PlayerParty> PlayerParties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Value Converter for PokemonType Enum -> String
        var pokemonTypeConverter = new ValueConverter<PokemonType, string>(
            v => v.ToString(),
            v => (PokemonType)Enum.Parse(typeof(PokemonType), v)
        );

        var pokemonTypeNullableConverter = new ValueConverter<PokemonType?, string?>(
            v => v.HasValue ? v.Value.ToString() : null,
            v => v != null ? (PokemonType)Enum.Parse(typeof(PokemonType), v) : null
        );

        // Player Entity Configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Player");
            entity.HasKey(p => p.PlayerId);
            entity.Property(p => p.PlayerId).HasColumnName("playerId").HasMaxLength(255);
            entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(p => p.IconUrl).HasColumnName("iconUrl").HasMaxLength(255);
        });

        // PokemonSpecies Entity Configuration
        modelBuilder.Entity<PokemonSpecies>(entity =>
        {
            entity.ToTable("PokemonSpecies");
            entity.HasKey(ps => ps.PokemonSpeciesId);
            entity.Property(ps => ps.PokemonSpeciesId).HasColumnName("pokemonSpeciesId").ValueGeneratedNever();
            entity.Property(ps => ps.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(ps => ps.FrontImage).HasColumnName("frontImage").HasMaxLength(255);
            entity.Property(ps => ps.BackImage).HasColumnName("backImage").HasMaxLength(255);
            entity.Property(ps => ps.Type1).HasColumnName("type1").HasMaxLength(63).HasConversion(pokemonTypeConverter);
            entity.Property(ps => ps.Type2).HasColumnName("type2").HasMaxLength(63).HasConversion(pokemonTypeNullableConverter);
            entity.Property(ps => ps.EvolveLevel).HasColumnName("evolveLevel");
            entity.Property(ps => ps.BaseHp).HasColumnName("baseHp");
            entity.Property(ps => ps.BaseAttack).HasColumnName("baseAttack");
            entity.Property(ps => ps.BaseDefence).HasColumnName("baseDefense");
            entity.Property(ps => ps.BaseSpecialAttack).HasColumnName("baseSpecialAttack");
            entity.Property(ps => ps.BaseSpecialDefence).HasColumnName("baseSpecialDefense");
            entity.Property(ps => ps.BaseSpeed).HasColumnName("baseSpeed");
            
            // PokemonSpecies.MoveList: Many-to-Many (PokemonMove中間テーブル)
            entity.HasMany(ps => ps.MoveList)
                  .WithMany()
                  .UsingEntity<Dictionary<string, object>>(
                      "PokemonMove",
                      j => j.HasOne<Move>().WithMany().HasForeignKey("moveId"),
                      j => j.HasOne<PokemonSpecies>().WithMany().HasForeignKey("pokemonSpeciesId"),
                      j =>
                      {
                          j.ToTable("PokemonMove");
                          j.HasKey("pokemonSpeciesId", "moveId");
                      });
        });

        // Move Entity Configuration
        modelBuilder.Entity<Move>(entity =>
        {
            entity.ToTable("Move");
            entity.HasKey(m => m.MoveId);
            entity.Property(m => m.MoveId).HasColumnName("moveId").ValueGeneratedNever();
            entity.Property(m => m.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(m => m.Type).HasColumnName("type").HasMaxLength(63).HasConversion(pokemonTypeConverter);
            entity.Property(m => m.Power).HasColumnName("power");
            entity.Property(m => m.Accuracy).HasColumnName("accuracy");
            entity.Property(m => m.Pp).HasColumnName("pp");
            entity.Property(m => m.Priority).HasColumnName("priority");
            entity.Property(m => m.Target).HasColumnName("target").HasMaxLength(63);
            entity.Property(m => m.StatChance).HasColumnName("statChance");
            entity.Property(m => m.Ailment).HasColumnName("ailment").HasMaxLength(63);
            entity.Property(m => m.AilmentChance).HasColumnName("ailmentChance");
            entity.Property(m => m.Healing).HasColumnName("healing");
            entity.Property(m => m.Drain).HasColumnName("drain");
            entity.Property(m => m.CritRate).HasColumnName("critRate");

            entity.OwnsMany(m => m.StatChanges, sc =>
            {
                sc.ToTable("MoveStatChange");
                sc.WithOwner().HasForeignKey("MoveId");
                sc.HasKey("MoveId", "Stat"); // 複合キーを明示的に設定
                sc.Property(s => s.Stat).HasColumnName("stat").HasConversion<string>();
                sc.Property(s => s.Change).HasColumnName("change");
            });
        });

        // Pokemon Entity Configuration
        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.ToTable("Pokemon");
            entity.HasKey(p => p.PokemonId);
            entity.Property(p => p.PokemonId).HasColumnName("pokemonId").HasMaxLength(255);
            entity.Property(p => p.PokemonSpeciesId).HasColumnName("pokemonSpeciesId");
            entity.Property(p => p.Level).HasColumnName("level");
            entity.Property(p => p.Exp).HasColumnName("exp");

            entity.HasOne(p => p.Species)
                  .WithMany()
                  .HasForeignKey(p => p.PokemonSpeciesId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Pokemon.Moves: One-to-Many (最大4つ)
            entity.HasMany(p => p.Moves)
                  .WithMany()
                  .UsingEntity<Dictionary<string, object>>(
                      "PokemonMove2", // 別名で混同を避ける
                      j => j.HasOne<Move>().WithMany().HasForeignKey("moveId"),
                      j => j.HasOne<Pokemon>().WithMany().HasForeignKey("pokemonId"),
                      j =>
                      {
                          j.ToTable("PokemonMoveInstance");
                          j.HasKey("pokemonId", "moveId");
                      });
        });

        // PlayerParty Entity Configuration
        modelBuilder.Entity<PlayerParty>(entity =>
        {
            entity.ToTable("PlayerParty");
            entity.HasKey(pp => pp.PlayerId);
            entity.Property(pp => pp.PlayerId).HasColumnName("playerId").HasMaxLength(255);

            // Playerへのナビゲーションプロパティは削除し、外部キー制約のみ保持
            // PlayerId列はPlayerテーブルへの外部キーとして機能するが、
            // ナビゲーションプロパティがないためEFは自動追跡を行わない

            entity.HasMany(pp => pp.Party)
                  .WithMany()
                  .UsingEntity<Dictionary<string, object>>(
                      "PlayerPartyPokemon",
                      j => j.HasOne<Pokemon>().WithMany().HasForeignKey("pokemonId"),
                      j => j.HasOne<PlayerParty>().WithMany().HasForeignKey("playerId"),
                      j =>
                      {
                          j.ToTable("PlayerPartyPokemon");
                          j.HasKey("playerId", "pokemonId");
                      });
        });
    }
}
