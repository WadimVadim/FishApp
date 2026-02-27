using System;
using System.Collections.Generic;

namespace fishapp.Models;

public partial class OrderProduct
{
    public int Id { get; set; }

    public int? IdOrder { get; set; }

    public int? IdProduct { get; set; }

    public int? Count { get; set; }

    public virtual Order? IdOrderNavigation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
