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

        return RedirectToAction("Payment", new { bookingId = booking.BookingId });
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

    // GET: /Booking/Payment/5
public IActionResult Payment(int bookingId)
{
    var booking = _context.Bookings
        .Include(b => b.Room).ThenInclude(r => r!.RoomType)
        .FirstOrDefault(b => b.BookingId == bookingId);

    if (booking == null) return NotFound();

    var vm = new PaymentViewModel
    {
        BookingId     = booking.BookingId,
        Amount        = booking.TotalPrice ?? 0,
        RoomName      = booking.Room?.RoomType?.TypeName,
        CheckIn       = booking.CheckInDate?.ToString("dd/MM/yyyy"),
        CheckOut      = booking.CheckOutDate?.ToString("dd/MM/yyyy"),
        PaymentMethod = "Cash"
    };

    return View(vm);
}

// POST: /Booking/Payment
[HttpPost]
public IActionResult Payment(PaymentViewModel vm)
{
    var payment = new kkkk11.Models.Db.Payment
    {
        BookingId     = vm.BookingId,
        Amount        = vm.Amount,
        PaymentMethod = vm.PaymentMethod,
        PaymentStatus = "Paid",
        PaymentDate   = DateTime.Now
    };

    _context.Payments.Add(payment);

    // อัปเดต booking status เป็น Confirmed
    var booking = _context.Bookings.Find(vm.BookingId);
    if (booking != null) booking.BookingStatus = "Confirmed";

    _context.SaveChanges();

    return RedirectToAction("PaymentSuccess", new { bookingId = vm.BookingId });
}

// GET: /Booking/PaymentSuccess
public IActionResult PaymentSuccess(int bookingId)
{
    var booking = _context.Bookings
        .Include(b => b.Room).ThenInclude(r => r!.RoomType)
        .FirstOrDefault(b => b.BookingId == bookingId);

    return View(booking);
}
}