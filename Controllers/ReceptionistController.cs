using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using kkkk11.Models.Db;

[Authorize(Roles = "Receptionist")]
public class ReceptionistController : Controller
{
    private readonly Csi402dbContext _context;

    public ReceptionistController(Csi402dbContext context)
    {
        _context = context;
    }

    // หน้าหลัก — ตรวจสอบการจองทั้งหมด
    public IActionResult BookingList()
    {
        var bookings = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room).ThenInclude(r => r!.RoomType)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();

        return View(bookings);
    }

    // ยกเลิกการจอง
    [HttpPost]
    public IActionResult Cancel(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();

        booking.BookingStatus = "Cancelled";
        _context.SaveChanges();

        return RedirectToAction("BookingList");
    }
}