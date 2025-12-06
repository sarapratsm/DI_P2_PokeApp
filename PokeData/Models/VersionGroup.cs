using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class VersionGroup
{
    public int VersionId { get; set; }

    public string VersionName { get; set; } = null!;

    public int? Order { get; set; }

    public virtual ICollection<PokemonMove> PokemonMoves { get; set; } = new List<PokemonMove>();
}
