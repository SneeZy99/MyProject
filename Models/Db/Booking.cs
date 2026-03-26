using System;
using System.Collections.Generic;

namespace kkkk11.Models.Db;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public DateOnly? CheckInDate { get; set; }

    public DateOnly? CheckOutDate { get; set; }

    public string? BookingStatus { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Checkinout> Checkinouts { get; set; } = new List<Checkinout>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room? Room { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<Promotion> Promos { get; set; } = new List<Promotion>();
}
