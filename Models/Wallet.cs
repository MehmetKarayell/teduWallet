using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Wallet
{
    public int WalletId { get; set; }

    public decimal Balance { get; set; }

    public int StudentId { get; set; }

    public DateOnly? LastUpdated { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual ICollection<WalletSpendsReward> WalletSpendsRewards { get; set; } = new List<WalletSpendsReward>();
}
