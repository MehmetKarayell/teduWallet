using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using teduWallet.Models;
using teduWallet.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace teduWallet.Controllers
{
  [Authorize(Roles = "Admin")]
  public class AdminController : Controller
  {
    private readonly CampuscoinContext _context;
    private readonly IActivityService _activityService;
    private static readonly object _fileLock = new object();
    private readonly string _adminFilePath;
    private readonly IWebHostEnvironment _env;
    private readonly IWalletService _walletService;

    public AdminController(CampuscoinContext context, IWebHostEnvironment env, IActivityService activityService, IWalletService walletService)
    {
      _context = context;
      _env = env;
      _adminFilePath = Path.Combine(env.ContentRootPath, "App_Data", "AdminList.txt");
      _activityService = activityService;
      _walletService = walletService;
    }

    public async Task<IActionResult> Dashboard()
    {
      ViewBag.TotalStudentsCount = await _activityService.GetTotalStudentsCount();
      ViewBag.ActiveActivitiesCount = await _activityService.GetActiveActivitiesCount();
      ViewBag.PendingCount = await _activityService.GetPendingApprovalsCount();
      ViewBag.FullName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Admin";

      var logs = await _walletService.GetAllLogs();
      ViewBag.Logs = logs;

      return View();
    }

    public async Task<IActionResult> PendingApprovals()
    {
      var pending = await _context.Applies
          .Include(a => a.Student)
          .Include(a => a.Activity)
          .Where(a => a.Status == "Applied")
          .ToListAsync();

      return View(pending);
    }

    //add students role to admin
    public async Task<IActionResult> Users()
    {
      var admins = await _context.Admins.ToListAsync();
      var students = await _context.Students.ToListAsync();

      var studentAdminStatus = new Dictionary<int, bool>();
      foreach (var student in students)
      {
        studentAdminStatus[student.StudentId] = IsUserAdmin(student.StudentId);
      }

      ViewBag.Admins = admins;
      ViewBag.Students = students;
      ViewBag.StudentAdminStatus = studentAdminStatus;

      return View();
    }

    [HttpPost]
    public IActionResult ChangeUserRole(int userId, bool isAdmin)
    {
      UpdateAdminFile(userId, isAdmin);
      return RedirectToAction("Users");
    }

    [HttpPost]
    public async Task<IActionResult> ApproveTask(int studentId, int activityId)
    {
      try
      {
        await _activityService.ApproveTask(studentId, activityId);
        TempData["Success"] = "Task approved and coins awarded!";
      }
      catch (Exception ex)
      {
        TempData["Error"] = ex.Message;
      }
      return RedirectToAction("PendingApprovals");
    }

    private bool IsUserAdmin(int userId)
    {
      if (!System.IO.File.Exists(_adminFilePath)) return false;

      lock (_fileLock)
      {
        var lines = System.IO.File.ReadAllLines(_adminFilePath);
        return lines.Contains(userId.ToString());
      }
    }

    // update admin file in app_data .txt file
    private void UpdateAdminFile(int userId, bool makeAdmin)
    {
      lock (_fileLock)
      {
        var ids = new HashSet<string>();
        if (System.IO.File.Exists(_adminFilePath))
        {
          ids = new HashSet<string>(System.IO.File.ReadAllLines(_adminFilePath));
        }

        if (makeAdmin)
        {
          ids.Add(userId.ToString());
        }
        else
        {
          ids.Remove(userId.ToString());
        }

        System.IO.File.WriteAllLines(_adminFilePath, ids);
      }
    }
  }
}
