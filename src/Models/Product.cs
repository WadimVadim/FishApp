using System;
using System.Collections.Generic;

namespace fishapp.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Article { get; set; }

    public string? Name { get; set; }

    public string? Unit { get; set; }

    public int? Price { get; set; }

    public int? MaxSale { get; set; }

    public int? IdProisvoditel { get; set; }

    public int? IdPostavshik { get; set; }

    public int? IdCategory { get; set; }

    public int? Sale { get; set; }

    public int? Count { get; set; }

    public string? Description { get; set; }

    public string? Imgpath { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

    public virtual Postavshik? IdPostavshikNavigation { get; set; }

    public virtual Proisvoditel? IdProisvoditelNavigation { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
