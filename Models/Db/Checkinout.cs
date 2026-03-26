using System;
using System.Collections.Generic;

namespace kkkk11.Models.Db;

public partial class Checkinout
{
    public int RecordId { get; set; }

    public int? BookingId { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public int? HandledBy { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User? HandledByNavigation { get; set; }
}
