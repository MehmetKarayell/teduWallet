using Microsoft.EntityFrameworkCore;
using teduWallet.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace teduWallet.Services
{
  public class ActivityService : IActivityService
  {
    private readonly CampuscoinContext _context;

    public ActivityService(CampuscoinContext context)
    {
      _context = context;
    }

    public async Task ApproveTask(int studentId, int activityId)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        // 1. Get activity to find reward amount
        var activity = await _context.Activities.FindAsync(activityId);
        if (activity == null)
          throw new Exception("Activity not found.");

        // 2. Update Application Status
        var application = await _context.Applies.FirstOrDefaultAsync(a => a.StudentId == studentId && a.ActivityId == activityId);
        if (application == null)
          throw new Exception("Application not found.");

        application.Status = "Approved";

        // 3. Check if already recorded in COMPLETES (redundancy check)
        var alreadyCompleted = await _context.Completes.AnyAsync(c => c.StudentId == studentId && c.ActivityId == activityId);
        if (alreadyCompleted)
          throw new Exception("Task already completed by this student.");

        // 4. Insert into COMPLETES
        var completion = new Complete
        {
          StudentId = studentId,
          ActivityId = activityId,
          CompletionDate = DateOnly.FromDateTime(DateTime.Now),
          AwardedAmount = activity.RewardTokenAmount
        };
        _context.Completes.Add(completion);

        // 5. Update STUDENT Coins
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
          throw new Exception("Student not found.");

        // Increase coins
        student.Coins = (student.Coins ?? 0) + activity.RewardTokenAmount;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task ApplyForTask(int studentId, int activityId)
    {
      var exists = await _context.Applies.AnyAsync(a => a.StudentId == studentId && a.ActivityId == activityId);
      if (exists)
        throw new Exception("Already applied for this task.");

      var application = new Apply
      {
        StudentId = studentId,
        ActivityId = activityId,
        Status = "Applied"
      };
      _context.Applies.Add(application);
      await _context.SaveChangesAsync();
    }

    public async Task<List<Activity>> GetActiveActivities()
    {
      var today = DateOnly.FromDateTime(DateTime.Now);
      return await _context.Activities
          .Where(a => (a.Status == "Active" || a.Status == "Open") && (a.Deadline == null || a.Deadline >= today))
          .Include(a => a.Admin)
          .ToListAsync();
    }
    public async Task<int> GetCompletedTasksCount(int studentId)
    {
      return await _context.Completes.CountAsync(c => c.StudentId == studentId);
    }

    public async Task<int> GetAvailableRewardsCount()
    {
      return await _context.Rewards.CountAsync(r => r.Status == "Active" || r.Status == "Available");
    }

    public async Task<int> GetTotalStudentsCount()
    {
      return await _context.Students.CountAsync();
    }

    public async Task<int> GetActiveActivitiesCount()
    {
      var today = DateOnly.FromDateTime(DateTime.Now);
      return await _context.Activities.CountAsync(a => (a.Status == "Active" || a.Status == "Open") && (a.Deadline == null || a.Deadline >= today));
    }

    public async Task<int> GetPendingApprovalsCount()
    {
      return await _context.Applies.CountAsync(a => a.Status == "Applied" || a.Status == "Pending");
    }

    public async Task<List<int>> GetAppliedActivityIds(int studentId)
    {
      return await _context.Applies
          .Where(a => a.StudentId == studentId)
          .Select(a => a.ActivityId)
          .ToListAsync();
    }
  }
}
