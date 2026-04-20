using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kkkk11.Models;
using kkkk11.ViewModels;
using kkkk11.Models.Db;

namespace kkkk11.Controllers;

public class HomeController : Controller
{
    private readonly Csi402dbContext _context;

    public HomeController(Csi402dbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Index()
{
    if (User.IsInRole("Admin"))
        return RedirectToAction("Dashboard", "Admin");

    if (User.IsInRole("Receptionist"))
        return RedirectToAction("Dashboard", "Receptionist");

    if (User.IsInRole("Staff"))
        return RedirectToAction("DailyReport", "Staff");

    return View();
}

    public IActionResult lab8()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult Userlist()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserViewModel data)
    {
        var user = _context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u =>
                u.Username == data.Username &&
                u.Password == data.Password);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Customer"),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Login failed";
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (_context.Users.Any(u => u.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Username นี้ถูกใช้แล้ว");
            return View(model);
        }

        if (_context.Users.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Email นี้ถูกใช้แล้ว");
            return View(model);
        }

        var user = new User
        {
            Username = model.Username,
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            Password = model.Password,
            RoleId = 2,
            Status = "Active"
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        TempData["Success"] = "สมัครสมาชิกสำเร็จ! กรุณาเข้าสู่ระบบ";
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
    // GET: /Home/EditProfile
    public IActionResult EditProfile()
    {
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return RedirectToAction("Login");
        return View(user);
    }

    // POST: /Home/EditProfile
    [HttpPost]
    public IActionResult EditProfile(User model)
    {
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return RedirectToAction("Login");

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.Phone = model.Phone;

        // เปลี่ยน password ถ้ากรอกมา
        if (!string.IsNullOrEmpty(model.Password))
            user.Password = model.Password;

        _context.SaveChanges();
        TempData["Success"] = "อัปเดตข้อมูลสำเร็จ!";
        return RedirectToAction("Index");
    }



}