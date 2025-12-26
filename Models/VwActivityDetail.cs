using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class VwActivityDetail
{
    public int ActivityId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public decimal RewardTokenAmount { get; set; }

    public int MaxParticipants { get; set; }

    public DateOnly? Deadline { get; set; }

    public string AdminName { get; set; } = null!;

    public string? AdminSurname { get; set; }
}
