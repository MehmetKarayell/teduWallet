using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string Position { get; set; } = null!;

    public string? Password { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
