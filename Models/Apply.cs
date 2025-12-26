using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Apply
{
    public int StudentId { get; set; }

    public int ActivityId { get; set; }

    public DateOnly ApplicationDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Activity Activity { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
