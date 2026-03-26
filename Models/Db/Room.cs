using System;
using System.Collections.Generic;

namespace kkkk11.Models.Db;

public partial class Room
{
    public int RoomId { get; set; }

    public string? RoomNumber { get; set; }

    public int? RoomTypeId { get; set; }

    public int? RoomStatusId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Roomstatus? RoomStatus { get; set; }

    public virtual Roomtype? RoomType { get; set; }
}
