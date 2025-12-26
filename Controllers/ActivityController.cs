using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using teduWallet.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace teduWallet.Controllers
{
  [Authorize(Roles = "Admin")]
  public class ActivityController : Controller
  {
    private readonly CampuscoinContext _context;

    public ActivityController(CampuscoinContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IActionResult AddActivity()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddActivity(Activity activity)
    {
      try
      {
        // 1. Manually generate ActivityId (Max + 1) to prevent conflicts as requested
        int maxId = await _context.Activities.AnyAsync()
                    ? await _context.Activities.MaxAsync(a => a.ActivityId)
                    : 0;
        activity.ActivityId = maxId + 1;

        // 2. Set AdminId based on logged in user (from Cookie Claims)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int adminId))
        {
            activity.AdminId = adminId;
        }
        else
        {
            ModelState.AddModelError("", "Admin session error. Could not identify logging admin.");
            return View(activity);
        }

        activity.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
        activity.Status = "Active";

        // Remove properties from validation that are not in the form
        ModelState.Remove("Admin");
        ModelState.Remove("AdminId");
        ModelState.Remove("ActivityId");
        ModelState.Remove("CreatedDate");
        ModelState.Remove("Status");

        if (ModelState.IsValid)
        {
          _context.Activities.Add(activity);
          await _context.SaveChangesAsync();

          // Redirect to Admin Dashboard on success as requested
          TempData["Success"] = "Activity '" + activity.Title + "' created successfully!";
          return RedirectToAction("Dashboard", "Admin");
        }
      }
      catch (Exception ex)
      {
        // Display error message on failure as requested
        ModelState.AddModelError("", "Failed to save activity: " + ex.Message);
      }

      return View(activity);
    }

    [HttpGet]
    public async Task<IActionResult> ManageActivities()
    {
      var today = DateOnly.FromDateTime(DateTime.Now);
      var activities = await _context.Activities
          .Where(a => a.Deadline == null || a.Deadline >= today)
          .OrderByDescending(a => a.CreatedDate)
          .ToListAsync();
      return View(activities);
    }

    [HttpGet]
    public async Task<IActionResult> EditActivity(int id)
    {
      var activity = await _context.Activities.FindAsync(id);
      if (activity == null) return NotFound();
      return View(activity);
    }

    [HttpPost]
    public async Task<IActionResult> EditActivity(Activity activity)
    {
        // We do not change ActivityId or CreatedDate or AdminId usually, but we need to keep them.
        // The form should include hidden fields or we re-fetch.
        // Best approach for EF update: fetch tracked entity and update values.

        try
        {
             var existing = await _context.Activities.FindAsync(activity.ActivityId);
             if (existing == null) return NotFound();

             existing.Title = activity.Title;
             existing.Description = activity.Description;
             existing.RewardTokenAmount = activity.RewardTokenAmount;
             existing.MaxParticipants = activity.MaxParticipants;
             existing.Deadline = activity.Deadline;
             // existing.PriorityLevel = activity.PriorityLevel; // Schema shows PriorityLevel is nullable decimal, View uses int.

             // If schema has PriorityLevel
             existing.PriorityLevel = activity.PriorityLevel;

             _context.Activities.Update(existing);
             await _context.SaveChangesAsync();

             TempData["Success"] = "Activity updated successfully!";
             return RedirectToAction("ManageActivities");
        }
        catch (Exception ex)
        {
             ModelState.AddModelError("", "Update failed: " + ex.Message);
        }
        return View(activity);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity != null)
        {
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Activity deleted.";
        }
        else
        {
             TempData["Error"] = "Activity not found.";
        }
        return RedirectToAction("ManageActivities");
    }
  }
}
