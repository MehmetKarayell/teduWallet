using Microsoft.AspNetCore.Mvc;
using teduWallet.Services;
using teduWallet.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace teduWallet.Controllers
{
  public class VerifyWalletController : Controller
  {
    private readonly IWalletService _walletService;
    private readonly CampuscoinContext _context;

    public VerifyWalletController(IWalletService walletService, CampuscoinContext context)
    {
      _walletService = walletService;
      _context = context;
    }

    public async Task<IActionResult> Index(int studentId = 8)
    {
      var student = await _context.Students.FindAsync(studentId);
      return Content($"Student ID: {studentId}, Name: {student?.Name}, Coins: {student?.Coins}");
    }



  }
}
