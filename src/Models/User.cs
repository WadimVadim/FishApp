using System;
using System.Collections.Generic;

namespace fishapp.Models;

public partial class User
{
    public int Id { get; set; }

    public int? IdRole { get; set; }

    public string? Fio { get; set; }

    public string? Login { get; set; }

    public string? Pass { get; set; }

    public virtual Role? IdRoleNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
