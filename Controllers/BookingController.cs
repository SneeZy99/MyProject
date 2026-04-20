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
    int nights = vm.CheckOutDate.DayNumber - vm.CheckInDate.DayNumber;
    if (nights <= 0)
    {
        ModelState.AddModelError("", "วันเช็คเอาท์ต้องมากกว่าวันเช็คอิน");
        return View(vm);
    }

    var username = User.Identity?.Name;
    var user = _context.Users.FirstOrDefault(u => u.Username == username);
    if (user == null) return RedirectToAction("Login", "Home");

    decimal basePrice = (vm.PricePerNight ?? 0) * nights;
    decimal discount  = 0;
    int?    promoId   = null;
    string  promoMsg  = "";

    // โปรโมชั่น 1: Early Bird จองล่วงหน้า 7 วัน
    var today = DateOnly.FromDateTime(DateTime.Today);
    if (vm.CheckInDate >= today.AddDays(7))
    {
        discount = basePrice * 0.05m;
        promoId  = 1;
        promoMsg = "Early Bird -5%";
    }

    // โปรโมชั่น 2 & 3: Stay 7 หรือ 10 วัน
    if (nights >= 10)
    {
        decimal d = basePrice * 0.10m;
        if (d > discount) { discount = d; promoId = 3; promoMsg = "Stay 10 Days -10%"; }
    }
    else if (nights >= 7)
    {
        decimal d = basePrice * 0.08m;
        if (d > discount) { discount = d; promoId = 2; promoMsg = "Stay 7 Days -8%"; }
    }

    // โปรโมชั่น 4 & 5: Loyal Customer
    var totalDays = _context.Bookings
        .Where(b => b.UserId == user.UserId && b.BookingStatus != "Cancelled")
        .ToList()
        .Sum(b => b.CheckOutDate.HasValue && b.CheckInDate.HasValue
            ? b.CheckOutDate.Value.DayNumber - b.CheckInDate.Value.DayNumber : 0);

    if (totalDays >= 30)
    {
        decimal d = basePrice * 0.15m;
        if (d > discount) { discount = d; promoId = 5; promoMsg = "Loyal 30 Days -15%"; }
    }
    else if (totalDays >= 15)
    {
        decimal d = basePrice * 0.10m;
        if (d > discount) { discount = d; promoId = 4; promoMsg = "Loyal 15 Days -10%"; }
    }

    decimal totalPrice = basePrice - discount;

    var booking = new Booking
    {
        UserId        = user.UserId,
        RoomId        = vm.RoomId,
        CheckInDate   = vm.CheckInDate,
        CheckOutDate  = vm.CheckOutDate,
        BookingStatus = "Pending",
        TotalPrice    = totalPrice,
        CreatedAt     = DateTime.Now
    };

    _context.Bookings.Add(booking);
    _context.SaveChanges();

    // เพิ่ม Promo ถ้ามี
    if (promoId.HasValue)
    {
        var promo = _context.Promotions.Find(promoId.Value);
        if (promo != null)
        {
            booking.Promos.Add(promo);
            _context.SaveChanges();
        }
    }

    TempData["PromoMsg"]  = promoMsg;
    TempData["Discount"]  = discount.ToString();
    TempData["BasePrice"] = basePrice.ToString();

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
        PaymentMethod = "Cash",
        PromoMsg      = TempData["PromoMsg"]?.ToString() ?? "",
        Discount      = decimal.TryParse(TempData["Discount"]?.ToString(), out var d) ? d : 0,
        BasePrice     = decimal.TryParse(TempData["BasePrice"]?.ToString(), out var bp) ? bp : 0
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