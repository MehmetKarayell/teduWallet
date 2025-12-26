using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using teduWallet.Models;

namespace teduWallet.Controllers
{
  public class AccountController : Controller
  {
    private readonly CampuscoinContext _context;
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _passwordHasher;

    public AccountController(CampuscoinContext context)
    {
      _context = context;
      _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
    }

    [HttpGet]
    public IActionResult Login()
    {
      if (User.Identity!.IsAuthenticated)
      {
        return RedirectToDashboard();
      }
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
      if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
      {
        // 1. Check Admin Table
        var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
        if (admin != null && !string.IsNullOrEmpty(admin.Password))
        {
          var result = _passwordHasher.VerifyHashedPassword(new object(), admin.Password, password);
          if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
          {
            string role = "Admin";
            await SignInUser(admin.Username, role, admin.Name + " " + admin.Surname, admin.AdminId);
            HttpContext.Session.SetString("Role", role);
            return RedirectToAction("Dashboard", "Admin");
          }
        }

        // 2. Check Student Table
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Username == username);
        if (student != null && !string.IsNullOrEmpty(student.Password))
        {
          var result = _passwordHasher.VerifyHashedPassword(new object(), student.Password, password);
          if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
          {
            string role = IsUserAdmin(student.StudentId) ? "Admin" : "Student";
            await SignInUser(student.Username, role, student.Name + " " + student.Surname, student.StudentId);
            HttpContext.Session.SetString("Role", role);

            if (role == "Admin")
              return RedirectToAction("Dashboard", "Admin");
            else
              return RedirectToAction("Dashboard", "Student");
          }
        }

        ModelState.AddModelError(string.Empty, "Invalid username or password");
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Username and Password are required");
      }

      return View();
    }

    private bool IsUserAdmin(int userId)
    {
      string filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "AdminList.txt");
      if (!System.IO.File.Exists(filePath)) return false;

      // Note: In a real app, use a shared service or static lock across controllers.
      // For this task, we'll follow the local implementation.
      try
      {
        var lines = System.IO.File.ReadAllLines(filePath);
        return lines.Contains(userId.ToString());
      }
      catch
      {
        return false;
      }
    }

    [HttpGet]
    public IActionResult Register()
    {
      if (User.Identity!.IsAuthenticated)
      {
        return RedirectToDashboard();
      }
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string? name, string? surname, string? email, string? username, string? department, string? position, string? password, string? role)
    {
      // Optional: Clear ModelState if you want to rely ONLY on manual validation
      // ModelState.Clear();

      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
      {
        ModelState.AddModelError(string.Empty, "Name, Email, Username, and Password are required.");
        return View();
      }

      // Check if username exists in either table
      var userExists = await _context.Admins.AnyAsync(a => a.Username == username) ||
                       await _context.Students.AnyAsync(s => s.Username == username);

      if (userExists)
      {
        ModelState.AddModelError(string.Empty, "Username is already taken.");
        return View();
      }

      string hashedPassword = _passwordHasher.HashPassword(new object(), password);

      if (role == "Admin")
      {
        var newAdmin = new Admin
        {
          Name = name,
          Surname = surname ?? "",
          Email = email,
          Username = username,
          Position = position ?? "Staff",
          Password = hashedPassword
        };
        _context.Admins.Add(newAdmin);
      }
      else
      {
        var newStudent = new Student
        {
          Name = name,
          Surname = surname ?? "",
          Email = email,
          Username = username,
          Departmant = department ?? "General", // Typo in DB 'Departmant'
          Password = hashedPassword,
          Coins = 0
        };
        _context.Students.Add(newStudent);
      }

      try
      {
        await _context.SaveChangesAsync();
        return RedirectToAction("Login");
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, $"Database Error: {ex.InnerException?.Message ?? ex.Message}");
      }

      return View();
    }

    private async Task SignInUser(string username, string role, string fullName, int userId)
    {
      var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("FullName", fullName)
            };

      var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

      var authProperties = new AuthenticationProperties
      {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
      };

      await HttpContext.SignInAsync(
          CookieAuthenticationDefaults.AuthenticationScheme,
          new ClaimsPrincipal(claimsIdentity),
          authProperties);
    }

    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
      return View();
    }

    private IActionResult RedirectToDashboard()
    {
      if (User.IsInRole("Admin"))
      {
        return RedirectToAction("Dashboard", "Admin");
      }
      else if (User.IsInRole("Student"))
      {
        return RedirectToAction("Dashboard", "Student");
      }
      return RedirectToAction("Index", "Home");
    }
  }
}
