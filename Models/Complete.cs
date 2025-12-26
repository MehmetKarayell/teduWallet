using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Complete
{
    public int StudentId { get; set; }

    public int ActivityId { get; set; }

    public DateOnly CompletionDate { get; set; }

    public decimal AwardedAmount { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
