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
        // ✅ แก้ bracket ที่ผิด — Include และ FirstOrDefault แยกกัน
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
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Customer")
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

        // เช็ค Username ซ้ำ
        if (_context.Users.Any(u => u.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Username นี้ถูกใช้แล้ว");
            return View(model);
        }

        // เช็ค Email ซ้ำ
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
            Password = model.Password,  // ควรทำ hash ในอนาคต
            RoleId = 4,               // 4 = Customer/User
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
}