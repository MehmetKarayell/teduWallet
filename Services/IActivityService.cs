using System.Collections.Generic;
using System.Threading.Tasks;
using teduWallet.Models;

namespace teduWallet.Services
{
  public interface IActivityService
  {
    Task ApproveTask(int studentId, int activityId);
    Task ApplyForTask(int studentId, int activityId);
    Task<List<Activity>> GetActiveActivities();
    Task<int> GetCompletedTasksCount(int studentId);
    Task<int> GetAvailableRewardsCount();
    Task<int> GetTotalStudentsCount();
    Task<int> GetActiveActivitiesCount();
    Task<int> GetPendingApprovalsCount();
    Task<List<int>> GetAppliedActivityIds(int studentId);
  }
}
