using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Log
{
    public int StudentId { get; set; }

    public int ActivityId { get; set; }

    public int? RewardId { get; set; }

    public string ActionType { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public int LogId { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Reward? Reward { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
