using Microsoft.AspNetCore.Mvc;
using kkkk11.ViewModels;
using kkkk11.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
public class BookingController : Controller
{
    private readonly Csi402dbContext _context;

    public BookingController(Csi402dbContext context)
    {
        _context = context;
    }

    // GET: /Booking/Create?roomId=1
    public IActionResult Create(int roomId)
    {
        var room = _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefault(r => r.RoomId == roomId);

        if (room == null) return NotFound();

        var vm = new BookingViewModel
        {
            RoomId       = room.RoomId,
            RoomName     = room.RoomType?.TypeName,
            PricePerNight = room.RoomType?.PricePerNight,
            CheckInDate  = DateOnly.FromDateTime(DateTime.Today),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
        };

        return View(vm);
    }

    // POST: /Booking/Create
    [HttpPost]
    public IActionResult Create(BookingViewModel vm)
    {
        // คำนวณจำนวนคืนและราคารวม
        int nights = vm.CheckOutDate.DayNumber - vm.CheckInDate.DayNumber;
        if (nights <= 0)
        {
            ModelState.AddModelError("", "วันเช็คเอาท์ต้องมากกว่าวันเช็คอิน");
            return View(vm);
        }

        // ดึง userId จาก session
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return RedirectToAction("Login", "Home");

        var booking = new Booking
        {
            UserId        = user.UserId,
            RoomId        = vm.RoomId,
            CheckInDate   = vm.CheckInDate,
            CheckOutDate  = vm.CheckOutDate,
            BookingStatus = "Pending",
            TotalPrice    = vm.PricePerNight * nights,
            CreatedAt     = DateTime.Now
        };

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }

    // GET: /Booking/MyBookings
    public IActionResult MyBookings()
    {
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return RedirectToAction("Login", "Home");

        var bookings = _context.Bookings
            .Include(b => b.Room)
                .ThenInclude(r => r!.RoomType)
            .Where(b => b.UserId == user.UserId)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();

        return View(bookings);
    }
}