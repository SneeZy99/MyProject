using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kkkk11.Models;
using kkkk11.ViewModels;
using kkkk11.Models.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace kkkk11.Controllers;

[Authorize]
public class RoomController : Controller
{
    private readonly Csi402dbContext _context;

    public RoomController(Csi402dbContext context)
    {
        _context = context;
    }

    public IActionResult Details(int id)
    {
        var room = _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.RoomStatus)
            .FirstOrDefault(r => r.RoomId == id);

        if (room == null) return NotFound();
        return View(room);
    }

    public IActionResult Index(string? checkIn, string? checkOut, int guests = 1)
{
    var rooms = _context.Rooms
        .Include(r => r.RoomType)
        .Include(r => r.RoomStatus)
        .Where(r => r.RoomType!.Capacity >= guests)
        .ToList();

    // กรองห้องที่ว่างตามวันที่
    if (!string.IsNullOrEmpty(checkIn) && !string.IsNullOrEmpty(checkOut))
    {
        var ci = DateOnly.Parse(checkIn);
        var co = DateOnly.Parse(checkOut);

        // หา room_id ที่ถูกจองช่วงนั้น
        var bookedRoomIds = _context.Bookings
            .Where(b => b.BookingStatus != "Cancelled"
                     && b.CheckInDate < co
                     && b.CheckOutDate > ci)
            .Select(b => b.RoomId)
            .ToList();

        rooms = rooms.Where(r => !bookedRoomIds.Contains(r.RoomId)).ToList();
    }

    ViewBag.CheckIn  = checkIn;
    ViewBag.CheckOut = checkOut;
    ViewBag.Guests   = guests;

    return View(rooms);
}
}
