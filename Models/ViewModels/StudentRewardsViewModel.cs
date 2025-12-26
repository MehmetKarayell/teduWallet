using System.Collections.Generic;
using teduWallet.Models;

namespace teduWallet.Models.ViewModels
{
    public class StudentRewardsViewModel
    {
        public decimal CurrentBalance { get; set; }
        public List<Reward> AvailableRewards { get; set; } = new List<Reward>();
        public List<WalletSpendsReward> MyRewards { get; set; } = new List<WalletSpendsReward>();
    }
}
