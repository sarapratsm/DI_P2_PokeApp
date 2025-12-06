using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonEvolution
{
    public int EvolId { get; set; }

    public int EvolvedSpeciesId { get; set; }

    public int? EvolMinimumLevel { get; set; }

    public virtual PokemonEvolutionMatchup EvolvedSpecies { get; set; } = null!;
}
