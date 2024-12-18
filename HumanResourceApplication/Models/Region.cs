using System;
using System.Collections.Generic;

namespace HumanResourceApplication.Models;

public partial class Region
{
    public decimal RegionId { get; set; }

    public string? RegionName { get; set; }

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();
}
