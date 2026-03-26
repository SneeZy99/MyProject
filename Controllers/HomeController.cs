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
         public IActionResult lab8()
        {
            var users = _context.Users.ToList();  
            return View(users);                   
        }


    public IActionResult Userlist()
    {
        return View();
    }

      [HttpPost]
    public IActionResult Login(LoginUserViewModel data)
    {
        string? a, b;
        a = data.Name;
        b = data.Password;
        @ViewBag.Name = a;
        @ViewBag.Password = b;

        return RedirectToAction("Index", "Home", new { Name = data.Name, Password = data.Password });
    }
    public IActionResult Index(string Name, string Password)
    {
        @ViewBag.Name = Name;
        @ViewBag.Password = Password;
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}