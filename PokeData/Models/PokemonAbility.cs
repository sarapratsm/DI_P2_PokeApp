using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class PokemonAbility
{
    public int PokId { get; set; }

    public int AbilId { get; set; }

    public bool IsHidden { get; set; }

    public int Slot { get; set; }

    public virtual Ability Abil { get; set; } = null!;

    public virtual Pokemon Pok { get; set; } = null!;
}
