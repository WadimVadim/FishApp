using System;
using System.Collections.Generic;

namespace fishapp.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly? DateOrder { get; set; }

    public DateOnly? DateDelivery { get; set; }

    public int? IdPunkt { get; set; }

    public int? IdUser { get; set; }

    public int? CodeDelivery { get; set; }

    public int? IdStatus { get; set; }

    public virtual Punkt? IdPunktNavigation { get; set; }

    public virtual Status? IdStatusNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
