using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class VwTop3StudentsThisWeek
{
    public int StudentId { get; set; }

    public string? FullName { get; set; }

    public decimal? WeeklyCoinsEarned { get; set; }
}
