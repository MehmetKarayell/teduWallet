using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Activity
{
    public int ActivityId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public decimal RewardTokenAmount { get; set; }

    public int MaxParticipants { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int AdminId { get; set; }

    public DateOnly? Deadline { get; set; }

    public decimal? PriorityLevel { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual ICollection<Apply> Applies { get; set; } = new List<Apply>();

    public virtual ICollection<Complete> Completes { get; set; } = new List<Complete>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
}
