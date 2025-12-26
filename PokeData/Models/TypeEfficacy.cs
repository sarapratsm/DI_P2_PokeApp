using System;
using System.Collections.Generic;

namespace PokeData.Models;

public partial class TypeEfficacy
{
    public int DamageTypeId { get; set; }

    public int TargetTypeId { get; set; }

    public int DamageFactor { get; set; }

    public virtual Type DamageType { get; set; } = null!;

    public virtual Type TargetType { get; set; } = null!;
}
