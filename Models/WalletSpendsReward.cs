using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class WalletSpendsReward
{
    public int TransactionId { get; set; }

    public int WalletId { get; set; }

    public int RewardId { get; set; }

    public DateOnly SpentDate { get; set; }

    public DateOnly UsedDate { get; set; }

    public int Quantity { get; set; }

    public int? StudentId { get; set; }

    public virtual Reward Reward { get; set; } = null!;

    public virtual Student? Student { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
