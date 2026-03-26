using System;
using System.Collections.Generic;

namespace kkkk11.Models.Db;

public partial class Roomstatus
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
