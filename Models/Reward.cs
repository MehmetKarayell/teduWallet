using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Reward
{
    public int RewardId { get; set; }

    public string RewardName { get; set; } = null!;

    public string RewardType { get; set; } = null!;

    public decimal Cost { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? Vendor { get; set; }

    public string UniqueCode { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateOnly? ExpiryDate { get; set; }

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<WalletSpendsReward> WalletSpendsRewards { get; set; } = new List<WalletSpendsReward>();
}
