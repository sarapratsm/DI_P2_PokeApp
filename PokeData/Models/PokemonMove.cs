using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonMove
{
    public int PokId { get; set; }

    public int VersionGroupId { get; set; }

    public int MoveId { get; set; }

    public int MethodId { get; set; }

    public int Level { get; set; }

    public virtual PokemonMoveMethod Method { get; set; } = null!;

    public virtual Move Move { get; set; } = null!;

    public virtual Pokemon Pok { get; set; } = null!;

    public virtual VersionGroup VersionGroup { get; set; } = null!;
}
