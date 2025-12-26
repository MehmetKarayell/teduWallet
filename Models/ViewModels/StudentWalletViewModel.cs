using System.Collections.Generic;
using teduWallet.Models;

namespace teduWallet.Models.ViewModels
{
    public class StudentWalletViewModel
    {
        public decimal Balance { get; set; }
        public List<Log> TransactionHistory { get; set; } = new List<Log>();
        public List<BalanceHistoryPoint> BalanceHistory { get; set; } = new List<BalanceHistoryPoint>();
    }
}
