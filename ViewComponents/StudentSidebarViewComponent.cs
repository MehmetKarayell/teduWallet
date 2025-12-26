using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using teduWallet.Services;

namespace teduWallet.ViewComponents
{
  public class StudentSidebarViewComponent : ViewComponent
  {
    private readonly IWalletService _walletService;
    private readonly IActivityService _activityService;

    public StudentSidebarViewComponent(IWalletService walletService, IActivityService activityService)
    {
      _walletService = walletService;
      _activityService = activityService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
      var userIdString = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier)?.Value;
      decimal balance = 0;
      int completedTasks = 0;

      if (int.TryParse(userIdString, out int studentId))
      {
        balance = await _walletService.GetBalance(studentId);
        completedTasks = await _activityService.GetCompletedTasksCount(studentId);
      }

      ViewBag.SidebarBalance = balance;
      ViewBag.SidebarCompletedTasks = completedTasks;
      ViewBag.FullName = ((ClaimsPrincipal)User).FindFirst("FullName")?.Value ?? ((ClaimsPrincipal)User).Identity?.Name ?? "Student";

      return View();
    }
  }
}
