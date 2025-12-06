using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class Move
{
    public int MoveId { get; set; }

    public string MoveName { get; set; } = null!;

    public int TypeId { get; set; }

    public short? MovePower { get; set; }

    public short? MovePp { get; set; }

    public short? MoveAccuracy { get; set; }

    public virtual ICollection<PokemonMove> PokemonMoves { get; set; } = new List<PokemonMove>();

    public virtual Type Type { get; set; } = null!;
}
