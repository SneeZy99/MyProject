using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using kkkk11.Models.Db;

[Authorize(Roles = "Staff")]
public class StaffController : Controller
{
    private readonly Csi402dbContext _context;

    public StaffController(Csi402dbContext context)
    {
        _context = context;
    }

    // Check-in / Check-out รายวัน
    public IActionResult DailyReport()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var checkIns = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r!.RoomType)
            .Where(b => b.CheckInDate == today && b.BookingStatus == "Confirmed")
            .ToList();

        var checkOuts = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r!.RoomType)
            .Where(b => b.CheckOutDate == today && b.BookingStatus == "Confirmed")
            .ToList();

        ViewBag.CheckIns  = checkIns;
        ViewBag.CheckOuts = checkOuts;
        ViewBag.Today     = today.ToString("dd/MM/yyyy");

        return View();
    }
}