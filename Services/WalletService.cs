using Microsoft.EntityFrameworkCore;
using teduWallet.Models;
using System;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;

namespace teduWallet.Services
{
  public class WalletService : IWalletService
  {
    private readonly CampuscoinContext _context;

    public WalletService(CampuscoinContext context)
    {
      _context = context;
    }

    public async Task AddCoinsWithTransaction(int studentId, decimal amount, int activityId = 1)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
          throw new Exception("Student not found.");

        // Update coins
        student.Coins = (student.Coins ?? 0) + amount;

        // Log entry
        var log = new Log
        {
          StudentId = studentId,
          ActivityId = activityId,
          RewardId = 1, // Default reward ID as requested by schema
          ActionType = "EARN",
          Timestamp = DateTime.Now
        };
        _context.Logs.Add(log);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task SubtractCoinsWithTransaction(int studentId, decimal amount, int rewardId = 1)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
          throw new Exception("Student not found.");

        decimal currentCoins = student.Coins ?? 0;
        if (currentCoins < amount)
          throw new Exception("Insufficient balance.");

        // Update coins
        student.Coins = currentCoins - amount;

        // Log entry
        var log = new Log
        {
          StudentId = studentId,
          ActivityId = 1, // Default activity ID as requested by schema
          RewardId = rewardId,
          ActionType = "SPEND",
          Timestamp = DateTime.Now
        };
        _context.Logs.Add(log);

        // Create WALLET_SPENDS_REWARD entry
        // Get wallet id
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.StudentId == studentId);
        if (wallet != null) 
        {
            int maxId = 0;
            if (await _context.WalletSpendsRewards.AnyAsync())
            {
                maxId = await _context.WalletSpendsRewards.MaxAsync(w => w.TransactionId);
            }
            
            var spend = new WalletSpendsReward
            {
                TransactionId = maxId + 1,
                WalletId = wallet.WalletId,
                RewardId = rewardId,
                SpentDate = DateOnly.FromDateTime(DateTime.Now),
                UsedDate = DateOnly.FromDateTime(DateTime.Now), // Assuming used immediately or placeholder
                Quantity = 1,
                StudentId = studentId
            };
            _context.WalletSpendsRewards.Add(spend);
        }
        else
        {
             // Create a wallet for the student if it doesn't exist
             int maxWalletId = await _context.Wallets.AnyAsync() ? await _context.Wallets.MaxAsync(w => w.WalletId) : 0;
             wallet = new Wallet
             {
                 WalletId = maxWalletId + 1,
                 StudentId = studentId,
                 Balance = 0 // Just creating the container
             };
             _context.Wallets.Add(wallet);
             await _context.SaveChangesAsync(); // Save locally to get ID if needed, though we set it manually

             int maxId = 0;
             if (await _context.WalletSpendsRewards.AnyAsync())
             {
                 maxId = await _context.WalletSpendsRewards.MaxAsync(w => w.TransactionId);
             }
            
             var spend = new WalletSpendsReward
             {
                 TransactionId = maxId + 1,
                 WalletId = wallet.WalletId,
                 RewardId = rewardId,
                 SpentDate = DateOnly.FromDateTime(DateTime.Now),
                 UsedDate = DateOnly.FromDateTime(DateTime.Now),
                 Quantity = 1,
                 StudentId = studentId
             };
             _context.WalletSpendsRewards.Add(spend);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
    public async Task<decimal> GetBalance(int studentId)
    {
      var student = await _context.Students.FindAsync(studentId);
      return student?.Coins ?? 0;
    }
    public async Task<List<VwTop3StudentsThisWeek>> GetTopStudentsThisWeek()
    {
      return await _context.VwTop3StudentsThisWeeks
          .OrderByDescending(s => s.WeeklyCoinsEarned)
          .ToListAsync();
    }

    public async Task<List<Reward>> GetAvailableRewards()
    {
      return await _context.Rewards
          .Where(r => r.Status == "Active" || r.Status == "Available")
          .ToListAsync();
    }

    public async Task<List<Log>> GetTransactionHistory(int studentId)
    {
      return await _context.Logs
          .Include(l => l.Activity)
          .Include(l => l.Reward)
          .Where(l => l.StudentId == studentId)
          .OrderByDescending(l => l.Timestamp)
          .ToListAsync();
    }

    public async Task<List<Log>> GetAllLogs()
    {
      return await _context.Logs
          .Include(l => l.Student)
          .Include(l => l.Activity)
          .Include(l => l.Reward)
          .OrderByDescending(l => l.Timestamp)
          .ToListAsync();
    }
    public Task<List<WalletSpendsReward>> GetStudentRewards(int studentId)
    {
      return _context.WalletSpendsRewards
          .Include(w => w.Reward)
          .Where(w => w.StudentId == studentId)
          .OrderByDescending(w => w.SpentDate)
          .ToListAsync();
    }
  }
}
