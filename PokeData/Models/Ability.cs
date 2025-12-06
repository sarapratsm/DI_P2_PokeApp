using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class Ability
{
    public int AbilId { get; set; }

    public string AbilName { get; set; } = null!;

    public virtual ICollection<PokemonAbility> PokemonAbilities { get; set; } = new List<PokemonAbility>();
}
