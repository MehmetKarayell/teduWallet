using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class VwStudentWallet
{
    public int StudentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string Username { get; set; } = null!;

    public int WalletId { get; set; }

    public decimal Balance { get; set; }

    public DateOnly? LastUpdated { get; set; }
}
