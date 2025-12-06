using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace PokeData.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ability> Abilities { get; set; }

    public virtual DbSet<BaseStat> BaseStats { get; set; }

    public virtual DbSet<Move> Moves { get; set; }

    public virtual DbSet<Pokemon> Pokemons { get; set; }

    public virtual DbSet<PokemonAbility> PokemonAbilities { get; set; }

    public virtual DbSet<PokemonEvolution> PokemonEvolutions { get; set; }

    public virtual DbSet<PokemonEvolutionMatchup> PokemonEvolutionMatchups { get; set; }

    public virtual DbSet<PokemonHabitat> PokemonHabitats { get; set; }

    public virtual DbSet<PokemonMove> PokemonMoves { get; set; }

    public virtual DbSet<PokemonMoveMethod> PokemonMoveMethods { get; set; }

    public virtual DbSet<PokemonType> PokemonTypes { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<TypeEfficacy> TypeEfficacies { get; set; }

    public virtual DbSet<VersionGroup> VersionGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=pokemon;port=3306;uid=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Ability>(entity =>
        {
            entity.HasKey(e => e.AbilId).HasName("PRIMARY");

            entity
                .ToTable("abilities")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AbilId)
                .HasColumnType("int(11)")
                .HasColumnName("abil_id");
            entity.Property(e => e.AbilName)
                .HasMaxLength(79)
                .HasColumnName("abil_name");
        });

        modelBuilder.Entity<BaseStat>(entity =>
        {
            entity.HasKey(e => e.PokId).HasName("PRIMARY");

            entity
                .ToTable("base_stats")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.PokId)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("pok_id");
            entity.Property(e => e.BAtk)
                .HasColumnType("int(11)")
                .HasColumnName("b_atk");
            entity.Property(e => e.BDef)
                .HasColumnType("int(11)")
                .HasColumnName("b_def");
            entity.Property(e => e.BHp)
                .HasColumnType("int(11)")
                .HasColumnName("b_hp");
            entity.Property(e => e.BSpAtk)
                .HasColumnType("int(11)")
                .HasColumnName("b_sp_atk");
            entity.Property(e => e.BSpDef)
                .HasColumnType("int(11)")
                .HasColumnName("b_sp_def");
            entity.Property(e => e.BSpeed)
                .HasColumnType("int(11)")
                .HasColumnName("b_speed");

            entity.HasOne(d => d.Pok).WithOne(p => p.BaseStat)
                .HasForeignKey<BaseStat>(d => d.PokId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pok_id");
        });

        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasKey(e => e.MoveId).HasName("PRIMARY");

            entity
                .ToTable("moves")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.TypeId, "type_id");

            entity.Property(e => e.MoveId)
                .HasColumnType("int(11)")
                .HasColumnName("move_id");
            entity.Property(e => e.MoveAccuracy)
                .HasColumnType("smallint(6)")
                .HasColumnName("move_accuracy");
            entity.Property(e => e.MoveName)
                .HasMaxLength(79)
                .HasColumnName("move_name");
            entity.Property(e => e.MovePower)
                .HasColumnType("smallint(6)")
                .HasColumnName("move_power");
            entity.Property(e => e.MovePp)
                .HasColumnType("smallint(6)")
                .HasColumnName("move_pp");
            entity.Property(e => e.TypeId)
                .HasColumnType("int(11)")
                .HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Moves)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("moves_ibfk_2");
        });

        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.HasKey(e => e.PokId).HasName("PRIMARY");

            entity
                .ToTable("pokemon")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.PokId)
                .HasColumnType("int(11)")
                .HasColumnName("pok_id");
            entity.Property(e => e.PokBaseExperience)
                .HasColumnType("int(11)")
                .HasColumnName("pok_base_experience");
            entity.Property(e => e.PokHeight)
                .HasColumnType("int(11)")
                .HasColumnName("pok_height");
            entity.Property(e => e.PokImage)
                .HasMaxLength(400)
                .HasColumnName("pok_image");
            entity.Property(e => e.PokName)
                .HasMaxLength(79)
                .HasColumnName("pok_name");
            entity.Property(e => e.PokWeight)
                .HasColumnType("int(11)")
                .HasColumnName("pok_weight");
        });

        modelBuilder.Entity<PokemonAbility>(entity =>
        {
            entity.HasKey(e => new { e.PokId, e.Slot })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("pokemon_abilities")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.AbilId, "abil_id");

            entity.HasIndex(e => e.IsHidden, "ix_pokemon_abilities_is_hidden");

            entity.Property(e => e.PokId)
                .HasColumnType("int(11)")
                .HasColumnName("pok_id");
            entity.Property(e => e.Slot)
                .HasColumnType("int(11)")
                .HasColumnName("slot");
            entity.Property(e => e.AbilId)
                .HasColumnType("int(11)")
                .HasColumnName("abil_id");
            entity.Property(e => e.IsHidden).HasColumnName("is_hidden");

            entity.HasOne(d => d.Abil).WithMany(p => p.PokemonAbilities)
                .HasForeignKey(d => d.AbilId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_abilities_ibfk_2");

            entity.HasOne(d => d.Pok).WithMany(p => p.PokemonAbilities)
                .HasForeignKey(d => d.PokId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_abilities_ibfk_1");
        });

        modelBuilder.Entity<PokemonEvolution>(entity =>
        {
            entity.HasKey(e => e.EvolId).HasName("PRIMARY");

            entity
                .ToTable("pokemon_evolution")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.EvolvedSpeciesId, "evolved_species_id");

            entity.Property(e => e.EvolId)
                .HasColumnType("int(11)")
                .HasColumnName("evol_id");
            entity.Property(e => e.EvolMinimumLevel)
                .HasColumnType("int(11)")
                .HasColumnName("evol_minimum_level");
            entity.Property(e => e.EvolvedSpeciesId)
                .HasColumnType("int(11)")
                .HasColumnName("evolved_species_id");

            entity.HasOne(d => d.EvolvedSpecies).WithMany(p => p.PokemonEvolutions)
                .HasForeignKey(d => d.EvolvedSpeciesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_evolution_ibfk_1");
        });

        modelBuilder.Entity<PokemonEvolutionMatchup>(entity =>
        {
            entity.HasKey(e => e.PokId).HasName("PRIMARY");

            entity
                .ToTable("pokemon_evolution_matchup")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.EvolvesFromSpeciesId, "evolves_from_species_id");

            entity.HasIndex(e => e.HabId, "habitat_id");

            entity.Property(e => e.PokId)
                .ValueGeneratedOnAdd()
                .HasColumnType("int(11)")
                .HasColumnName("pok_id");
            entity.Property(e => e.BaseHappiness)
                .HasColumnType("int(11)")
                .HasColumnName("base_happiness");
            entity.Property(e => e.CaptureRate)
                .HasColumnType("int(11)")
                .HasColumnName("capture_rate");
            entity.Property(e => e.EvolvesFromSpeciesId)
                .HasColumnType("int(11)")
                .HasColumnName("evolves_from_species_id");
            entity.Property(e => e.GenderRate)
                .HasColumnType("int(11)")
                .HasColumnName("gender_rate");
            entity.Property(e => e.HabId)
                .HasColumnType("int(11)")
                .HasColumnName("hab_id");

            entity.HasOne(d => d.Hab).WithMany(p => p.PokemonEvolutionMatchups)
                .HasForeignKey(d => d.HabId)
                .HasConstraintName("pokemon_evolution_matchup_ibfk_6");

            entity.HasOne(d => d.Pok).WithOne(p => p.PokemonEvolutionMatchup)
                .HasForeignKey<PokemonEvolutionMatchup>(d => d.PokId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("poke_fk");
        });

        modelBuilder.Entity<PokemonHabitat>(entity =>
        {
            entity.HasKey(e => e.HabId).HasName("PRIMARY");

            entity
                .ToTable("pokemon_habitats")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.HabId)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("hab_id");
            entity.Property(e => e.HabDescript)
                .HasMaxLength(400)
                .HasColumnName("hab_descript");
            entity.Property(e => e.HabName)
                .HasMaxLength(79)
                .HasColumnName("hab_name");
        });

        modelBuilder.Entity<PokemonMove>(entity =>
        {
            entity.HasKey(e => new { e.PokId, e.VersionGroupId, e.MoveId, e.MethodId, e.Level })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0, 0 });

            entity
                .ToTable("pokemon_moves")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Level, "ix_pokemon_moves_level");

            entity.HasIndex(e => e.MoveId, "ix_pokemon_moves_move_id");

            entity.HasIndex(e => e.PokId, "ix_pokemon_moves_pokemon_id");

            entity.HasIndex(e => e.MethodId, "ix_pokemon_moves_pokemon_move_method_id");

            entity.HasIndex(e => e.VersionGroupId, "ix_pokemon_moves_version_group_id");

            entity.Property(e => e.PokId)
                .HasColumnType("int(11)")
                .HasColumnName("pok_id");
            entity.Property(e => e.VersionGroupId)
                .HasColumnType("int(11)")
                .HasColumnName("version_group_id");
            entity.Property(e => e.MoveId)
                .HasColumnType("int(11)")
                .HasColumnName("move_id");
            entity.Property(e => e.MethodId)
                .HasColumnType("int(11)")
                .HasColumnName("method_id");
            entity.Property(e => e.Level)
                .HasColumnType("int(11)")
                .HasColumnName("level");

            entity.HasOne(d => d.Method).WithMany(p => p.PokemonMoves)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_moves_ibfk_4");

            entity.HasOne(d => d.Move).WithMany(p => p.PokemonMoves)
                .HasForeignKey(d => d.MoveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_moves_ibfk_2");

            entity.HasOne(d => d.Pok).WithMany(p => p.PokemonMoves)
                .HasForeignKey(d => d.PokId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_moves_ibfk_1");

            entity.HasOne(d => d.VersionGroup).WithMany(p => p.PokemonMoves)
                .HasForeignKey(d => d.VersionGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_moves_ibfk_3");
        });

        modelBuilder.Entity<PokemonMoveMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PRIMARY");

            entity
                .ToTable("pokemon_move_methods")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.MethodId)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("method_id");
            entity.Property(e => e.MethodName)
                .HasMaxLength(79)
                .HasColumnName("method_name");
        });

        modelBuilder.Entity<PokemonType>(entity =>
        {
            entity.HasKey(e => new { e.PokId, e.Slot })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("pokemon_types")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.TypeId, "type_id");

            entity.Property(e => e.PokId)
                .HasColumnType("int(11)")
                .HasColumnName("pok_id");
            entity.Property(e => e.Slot)
                .HasColumnType("int(11)")
                .HasColumnName("slot");
            entity.Property(e => e.TypeId)
                .HasColumnType("int(11)")
                .HasColumnName("type_id");

            entity.HasOne(d => d.Pok).WithMany(p => p.PokemonTypes)
                .HasForeignKey(d => d.PokId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_types_ibfk_1");

            entity.HasOne(d => d.Type).WithMany(p => p.PokemonTypes)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pokemon_types_ibfk_2");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PRIMARY");

            entity
                .ToTable("types")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.DamageTypeId, "damage_type_idx");

            entity.Property(e => e.TypeId)
                .HasColumnType("int(11)")
                .HasColumnName("type_id");
            entity.Property(e => e.DamageTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("damage_type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(79)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<TypeEfficacy>(entity =>
        {
            entity.HasKey(e => new { e.DamageTypeId, e.TargetTypeId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("type_efficacy")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.TargetTypeId, "target_type_id");

            entity.Property(e => e.DamageTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("damage_type_id");
            entity.Property(e => e.TargetTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("target_type_id");
            entity.Property(e => e.DamageFactor)
                .HasColumnType("int(11)")
                .HasColumnName("damage_factor");
        });

        modelBuilder.Entity<VersionGroup>(entity =>
        {
            entity.HasKey(e => e.VersionId).HasName("PRIMARY");

            entity
                .ToTable("version_groups")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.VersionName, "identifier").IsUnique();

            entity.Property(e => e.VersionId)
                .HasColumnType("int(11)")
                .HasColumnName("version_id");
            entity.Property(e => e.Order)
                .HasColumnType("int(11)")
                .HasColumnName("order");
            entity.Property(e => e.VersionName)
                .HasMaxLength(79)
                .HasColumnName("version_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
