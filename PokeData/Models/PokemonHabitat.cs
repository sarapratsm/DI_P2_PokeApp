using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonHabitat
{
    public int HabId { get; set; }

    public string HabName { get; set; } = null!;

    public string? HabDescript { get; set; }

    public virtual ICollection<PokemonEvolutionMatchup> PokemonEvolutionMatchups { get; set; } = new List<PokemonEvolutionMatchup>();
}
