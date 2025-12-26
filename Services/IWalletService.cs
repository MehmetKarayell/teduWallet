using System.Threading.Tasks;
using System.Collections.Generic;
using teduWallet.Models;

namespace teduWallet.Services
{
  public interface IWalletService
  {
    /// <summary>
    /// Adds coins to a student's account within a transaction.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="amount">The amount of coins to add.</param>
    /// <param name="activityId">The ID of the activity (required for logging). Default is 1.</param>
    Task AddCoinsWithTransaction(int studentId, decimal amount, int activityId = 1);

    /// <summary>
    /// Subtracts coins from a student's account within a transaction.
    /// Ensures the balance does not fall below zero.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="amount">The amount of coins to subtract.</param>
    /// <param name="rewardId">The ID of the reward (required for logging). Default is 1.</param>
    Task SubtractCoinsWithTransaction(int studentId, decimal amount, int rewardId = 1);
    Task<decimal> GetBalance(int studentId);
    Task<List<VwTop3StudentsThisWeek>> GetTopStudentsThisWeek();
    Task<List<Reward>> GetAvailableRewards();
    Task<List<Log>> GetTransactionHistory(int studentId);
    Task<List<Log>> GetAllLogs();
    Task<List<WalletSpendsReward>> GetStudentRewards(int studentId);
  }
}
