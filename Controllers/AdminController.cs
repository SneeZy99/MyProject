using Microsoft.AspNetCore.Mvc;
using kkkk11.ViewModels;
using kkkk11.Models.Db;
using Microsoft.EntityFrameworkCore;
public class AdminController : Controller
{
    private readonly Csi402dbContext _context;

    public AdminController(Csi402dbContext context)
    {
        _context = context;
    }


    public IActionResult UserList()
    {
       var users = _context.Users
        .Include(u => u.Role)   
        .ToList();
        return View(users);
    }


    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddUser(AddUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var user = new User
        {
            Username = model.Username,
            Password = model.Password,
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            RoleId = model.RoleId,
            Status = "Active"
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return RedirectToAction("UserList");
    }

    [HttpGet]
    public IActionResult EditUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
            return NotFound();
        return View(user);   
    }
    [HttpPost]
    public IActionResult EditUser(User model)
    {
        var user = _context.Users.Find(model.UserId);

        if (user == null)
            return NotFound();
        user.Username = model.Username;
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.Phone = model.Phone;
        user.RoleId = model.RoleId;
        user.Password = model.Password;
        _context.SaveChanges();
        return RedirectToAction("UserList");
    }

    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        _context.SaveChanges();

        return RedirectToAction("UserList");
    }
}
