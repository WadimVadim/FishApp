using System;
using System.Collections.Generic;

namespace fishapp.Models;

public partial class Proisvoditel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
