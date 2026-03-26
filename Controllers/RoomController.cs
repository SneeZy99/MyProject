using Microsoft.AspNetCore.Mvc;
using kkkk11.Models.Db;

public class RoomController : Controller
{
    private readonly Csi402dbContext _context;

    public RoomController(Csi402dbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var rooms = _context.Rooms.ToList();
        return View(rooms);
    }
    public IActionResult Details(int id)
    {
        var room = _context.Rooms
            .FirstOrDefault(r => r.RoomId == id);

        if (room == null)
            return NotFound();

        return View(room);
    }

    [HttpPost]
    public IActionResult Book(int roomId)
    {
        var booking = new Booking()
        {
            RoomId = roomId,
            BookingDate = DateTime.Now
        };

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


}
