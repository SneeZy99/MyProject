using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using kkkk11.Models.Db;
using kkkk11.ViewModels;

[Authorize(Roles = "Receptionist")]
public class ReceptionistController : Controller
{
    private readonly Csi402dbContext _context;

    public ReceptionistController(Csi402dbContext context)
    {
        _context = context;
    }
    public IActionResult BookingList()
    {
        var bookings = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r!.RoomType)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
        return View(bookings);
    }

    [HttpPost]
    public IActionResult Confirm(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();
        booking.BookingStatus = "Confirmed";
        _context.SaveChanges();
        return RedirectToAction("BookingList");
    }

    [HttpPost]
    public IActionResult Cancel(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();
        booking.BookingStatus = "Cancelled";
        _context.SaveChanges();
        return RedirectToAction("BookingList");
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
        if (!ModelState.IsValid) return View(model);

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
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(User model)
    {
        var user = _context.Users.Find(model.UserId);
        if (user == null) return NotFound();

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

    public IActionResult Dashboard()
    {
        ViewBag.TotalUsers = _context.Users.Count();
        ViewBag.TotalRooms = _context.Rooms.Count();
        ViewBag.TotalBookings = _context.Bookings.Count();
        ViewBag.TotalRevenue = _context.Bookings
            .Where(b => b.BookingStatus == "Confirmed")
            .Sum(b => b.TotalPrice) ?? 0;
        ViewBag.Confirmed = _context.Bookings.Count(b => b.BookingStatus == "Confirmed");
        ViewBag.Cancelled = _context.Bookings.Count(b => b.BookingStatus == "Cancelled");

        ViewBag.RecentBookings = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r!.RoomType)
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .ToList();

        return View();
    }
}