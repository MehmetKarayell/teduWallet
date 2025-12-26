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

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            var student = await _context.Students.FindAsync(userId);
            if (student == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Users");
            }

            // 1. Remove related WalletSpendsReward records (by StudentId)
            // Use ExecuteDeleteAsync to avoid fetching entities that might cause 'Data is Null' errors
            await _context.WalletSpendsRewards
                .Where(w => w.StudentId == userId)
                .ExecuteDeleteAsync();

            // 2. Remove Wallet and its related records
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.StudentId == userId);
            if (wallet != null)
            {
                // Remove spending records linked by WalletId
                await _context.WalletSpendsRewards
                    .Where(w => w.WalletId == wallet.WalletId)
                    .ExecuteDeleteAsync();
                
                // Remove the Wallet itself
                _context.Wallets.Remove(wallet);
            }

            // 3. Remove Logs
            await _context.Logs
                .Where(l => l.StudentId == userId)
                .ExecuteDeleteAsync();

            // 4. Remove Applies
            await _context.Applies
                .Where(a => a.StudentId == userId)
                .ExecuteDeleteAsync();

            // 5. Remove Completes
            await _context.Completes
                .Where(c => c.StudentId == userId)
                .ExecuteDeleteAsync();

            // 6. Finally remove Student
            _context.Students.Remove(student);

            await _context.SaveChangesAsync();
            
            // Also remove from admin file if they are an admin
            UpdateAdminFile(userId, false);

            TempData["Success"] = "User deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error deleting user: " + ex.Message;
        }

        return RedirectToAction("Users");
    }
  }
}
