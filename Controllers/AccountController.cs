using Microsoft.AspNetCore.Mvc;
using kkkk11.ViewModels;
using System.Security.Cryptography.X509Certificates;

namespace kkkk11.Controllers;

public class AccountController : Controller
{
    public IActionResult Lab5()
    {
        //var User = new LabUserViewModel();
        //User.UserId = "aa";
        // var user = new List<LabUserViewModel>
        // {
        //     new LabUserViewModel {UserId = "A" , Name = "aa", Lastname = "a8"},
        //     new LabUserViewModel {UserId = "B" , Name = "bb", Lastname = "b9"},
        //     new LabUserViewModel {UserId = "C" , Name = "cc", Lastname = "c7"},
        // };
        // return View(User);
        return View();


    }
    [HttpPost]
        public IActionResult Lab5(LabUserViewModel data)
        {
            string? a,b,c;
            a = data.UserId;
            b = data.Name;
            c = data.Lastname;
            @ViewBag.UserId = a;
            @ViewBag.Name = b;
            @ViewBag.Lastname = c;
            return RedirectToAction("Lab51","Account",new{UserId = data.UserId , Name = data.Name , Lastname = data.Lastname});
            //return View();
        }
        public IActionResult Lab51(string UserId, string Name ,  string Lastname)
        {
            @ViewBag.UserId = UserId;
            @ViewBag.Name = Name;
            @ViewBag.Lastname = Lastname;
            return View();
        }

}
