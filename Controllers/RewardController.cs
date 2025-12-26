using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using teduWallet.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace teduWallet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RewardController : Controller
    {
        private readonly CampuscoinContext _context;

        public RewardController(CampuscoinContext context)
        {
            _context = context;
        }

        // Page 1: Create new reward
        [HttpGet]
        public IActionResult AddReward()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReward(Reward reward)
        {
            try
            {
                // 1. Manually generate RewardId (Max + 1)
                int maxId = await _context.Rewards.AnyAsync()
                            ? await _context.Rewards.MaxAsync(r => r.RewardId)
                            : 0;
                reward.RewardId = maxId + 1;

                reward.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
                reward.Status = "Active";

                // Remove properties from validation that are not in the form but might cause issues
                ModelState.Remove("RewardId");
                ModelState.Remove("CreatedDate");
                ModelState.Remove("Status");
                ModelState.Remove("Logs");
                ModelState.Remove("WalletSpendsRewards");

                if (ModelState.IsValid)
                {
                    _context.Rewards.Add(reward);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Reward '" + reward.RewardName + "' created successfully!";
                    return RedirectToAction("ManageRewards");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to save reward: " + ex.Message);
            }

            return View(reward);
        }

        // Page 3: List all rewards
        [HttpGet]
        public async Task<IActionResult> ManageRewards()
        {
            var rewards = await _context.Rewards
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
            return View(rewards);
        }

        // Page 2: Edit a reward
        [HttpGet]
        public async Task<IActionResult> EditReward(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward == null) return NotFound();
            return View(reward);
        }

        [HttpPost]
        public async Task<IActionResult> EditReward(Reward reward)
        {
            try
            {
                var existing = await _context.Rewards.FindAsync(reward.RewardId);
                if (existing == null) return NotFound();

                existing.RewardName = reward.RewardName;
                existing.RewardType = reward.RewardType;
                existing.Cost = reward.Cost;
                existing.Vendor = reward.Vendor;
                existing.UniqueCode = reward.UniqueCode;
                existing.ExpiryDate = reward.ExpiryDate;
                // Status can be managed via separate actions or here if added to form, 
                // but usually Edit keeps current status unless explicitly changed.
                // Assuming we might want to allow status edit if it's in the form, 
                // but based on "visually similar to create", create doesn't have status.
                
                _context.Rewards.Update(existing);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Reward updated successfully!";
                return RedirectToAction("ManageRewards");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Update failed: " + ex.Message);
            }
            return View(reward);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReward(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward != null)
            {
                // Check dependencies if needed, or catch FK constraint errors
                try 
                {
                    _context.Rewards.Remove(reward);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Reward deleted.";
                }
                catch
                {
                    TempData["Error"] = "Cannot delete reward because it has been used or referenced.";
                }
            }
            else
            {
                TempData["Error"] = "Reward not found.";
            }
            return RedirectToAction("ManageRewards");
        }
    }
}
