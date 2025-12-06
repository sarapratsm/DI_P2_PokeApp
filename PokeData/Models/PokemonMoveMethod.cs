using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonMoveMethod
{
    public int MethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public virtual ICollection<PokemonMove> PokemonMoves { get; set; } = new List<PokemonMove>();
}
