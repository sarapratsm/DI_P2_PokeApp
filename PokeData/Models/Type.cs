using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class Type
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public int? DamageTypeId { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Move> Moves { get; set; } = new List<Move>();

    public virtual ICollection<PokemonType> PokemonTypes { get; set; } = new List<PokemonType>();
}
