using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class BaseStat
{
    public int PokId { get; set; }

    public int? BHp { get; set; }

    public int? BAtk { get; set; }

    public int? BDef { get; set; }

    public int? BSpAtk { get; set; }

    public int? BSpDef { get; set; }

    public int? BSpeed { get; set; }

    public virtual Pokemon Pok { get; set; } = null!;
}
