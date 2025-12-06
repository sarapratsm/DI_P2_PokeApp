using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonType
{
    public int PokId { get; set; }

    public int TypeId { get; set; }

    public int Slot { get; set; }

    public virtual Pokemon Pok { get; set; } = null!;

    public virtual Type Type { get; set; } = null!;
}
