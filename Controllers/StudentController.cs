using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using teduWallet.Services;
using System.Security.Claims;
using teduWallet.Models;

using teduWallet.Models.ViewModels;

namespace teduWallet.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : Controller
  {
    private readonly IActivityService _activityService;
    private readonly IWalletService _walletService;

    public StudentController(IActivityService activityService, IWalletService walletService)
    {
      _activityService = activityService;
      _walletService = walletService;
    }

    public async Task<IActionResult> Dashboard()
    {
      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdString, out int studentId))
      {
        var viewModel = new StudentDashboardViewModel
        {
            Balance = await _walletService.GetBalance(studentId),
            CompletedTasksCount = await _activityService.GetCompletedTasksCount(studentId),
            AvailableRewardsCount = await _activityService.GetAvailableRewardsCount(),
            FullName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Student",
            ActiveActivities = await _activityService.GetActiveActivities(),
            TopStudents = await _walletService.GetTopStudentsThisWeek()
        };

        // Keep ViewBag for layout if needed, though mostly replaced by Model
        ViewBag.Balance = viewModel.Balance;
        ViewBag.CompletedTasksCount = viewModel.CompletedTasksCount;
        ViewBag.AvailableRewardsCount = viewModel.AvailableRewardsCount;

        return View(viewModel);
      }
      return Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> Apply(int activityId)
    {
      var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdString, out int studentId))
      {
        try
        {
          await _activityService.ApplyForTask(studentId, activityId);
          TempData["Success"] = "Successfully applied for the task!";
          return RedirectToAction("Tasks");
        }
        catch (Exception ex)
        {
          TempData["Error"] = ex.Message;
          return RedirectToAction("Tasks");
        }
      }
      return Unauthorized();
    }

    public async Task<IActionResult> Tasks()
    {
      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdString, out int studentId))
      {
          var activities = await _activityService.GetActiveActivities();
          var appliedIds = await _activityService.GetAppliedActivityIds(studentId);
          
          var viewModel = new StudentTasksViewModel
          {
              AvailableActivities = activities,
              AppliedActivityIds = appliedIds
          };
          return View(viewModel);
      }
      return Unauthorized();
    }

    public async Task<IActionResult> Rewards()
    {
      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdString, out int studentId))
      {
          var rewards = await _walletService.GetAvailableRewards();
          var balance = await _walletService.GetBalance(studentId);
          var myRewards = await _walletService.GetStudentRewards(studentId);

          var viewModel = new StudentRewardsViewModel
          {
              AvailableRewards = rewards,
              CurrentBalance = balance,
              MyRewards = myRewards
          };
          return View(viewModel);
      }
      return Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> RedeemReward(int rewardId)
    {
      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdString, out int studentId))
      {
         try
         {
             var rewards = await _walletService.GetAvailableRewards();
             var reward = rewards.FirstOrDefault(r => r.RewardId == rewardId);
             if (reward == null)
             {
                 TempData["Error"] = "Reward not found.";
             }
             else
             {
                 await _walletService.SubtractCoinsWithTransaction(studentId, reward.Cost, rewardId);
                 TempData["Success"] = "Reward redeemed successfully!";
                 TempData["RedeemedRewardId"] = rewardId;
             }
         }
         catch (Exception ex)
         {
             TempData["Error"] = "Redemption failed: " + ex.Message;
         }
         return RedirectToAction("Rewards");
      }
      return Unauthorized();
    }

    public async Task<IActionResult> Wallet()
    {
      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdString, out int studentId))
      {
          var balance = await _walletService.GetBalance(studentId);
          var history = await _walletService.GetTransactionHistory(studentId);
          var balanceHistory = await _walletService.GetBalanceHistory(studentId);

          var viewModel = new StudentWalletViewModel
          {
              Balance = balance,
              TransactionHistory = history,
              BalanceHistory = balanceHistory
          };
          return View(viewModel);
      }
      return Unauthorized();
    }

    public IActionResult Settings()
    {
      return View();
    }
  }
}
