using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class Pokemon
{
    public int PokId { get; set; }

    public string PokName { get; set; } = null!;

    public int? PokHeight { get; set; }

    public int? PokWeight { get; set; }

    public int? PokBaseExperience { get; set; }

    public string? PokImage { get; set; }

    public virtual BaseStat? BaseStat { get; set; }

    public virtual ICollection<PokemonAbility> PokemonAbilities { get; set; } = new List<PokemonAbility>();

    public virtual PokemonEvolutionMatchup? PokemonEvolutionMatchup { get; set; }

    public virtual ICollection<PokemonMove> PokemonMoves { get; set; } = new List<PokemonMove>();

    public virtual ICollection<PokemonType> PokemonTypes { get; set; } = new List<PokemonType>();
}
