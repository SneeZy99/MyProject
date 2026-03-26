using System;
using System.Collections.Generic;

namespace kkkk11.Models.Db;

public partial class Promotion
{
    public int PromoId { get; set; }

    public string? PromoName { get; set; }

    public int? DiscountPercent { get; set; }

    public string? ConditionText { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
