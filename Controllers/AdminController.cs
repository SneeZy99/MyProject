using Microsoft.AspNetCore.Mvc;
using kkkk11.ViewModels;

public class AdminController : Controller
{
    public IActionResult UserList()
    {
        return View();
    }


    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Lab5(LabUserViewModel data)
    {
        string? a, b, c;
        a = data.UserId;
        b = data.Name;
        c = data.Lastname;
        @ViewBag.UserId = a;
        @ViewBag.Name = b;
        @ViewBag.Lastname = c;
        return RedirectToAction("Lab51", "Account", new { UserId = data.UserId, Name = data.Name, Lastname = data.Lastname });
        //return View();
    }
}
