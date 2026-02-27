using System;
using System.Collections.Generic;

namespace fishapp.Models;

public partial class Punkt
{
    public int Id { get; set; }

    public string? Adress { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
