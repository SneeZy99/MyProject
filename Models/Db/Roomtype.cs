using System;
using System.Collections.Generic;

namespace kkkk11.Models.Db;

public partial class Roomtype
{
    public int RoomTypeId { get; set; }

    public string? TypeName { get; set; }

    public decimal? PricePerNight { get; set; }

    public int? Capacity { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
