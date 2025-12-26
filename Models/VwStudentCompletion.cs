using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class VwStudentCompletion
{
    public int StudentId { get; set; }

    public int ActivityId { get; set; }

    public DateOnly CompletionDate { get; set; }

    public decimal AwardedAmount { get; set; }
}
