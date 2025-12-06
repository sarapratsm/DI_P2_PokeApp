using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonEvolutionMatchup
{
    public int PokId { get; set; }

    public int? EvolvesFromSpeciesId { get; set; }

    public int? HabId { get; set; }

    public int GenderRate { get; set; }

    public int CaptureRate { get; set; }

    public int BaseHappiness { get; set; }

    public virtual PokemonHabitat? Hab { get; set; }

    public virtual Pokemon Pok { get; set; } = null!;

    public virtual ICollection<PokemonEvolution> PokemonEvolutions { get; set; } = new List<PokemonEvolution>();
}
