namespace kkkk11.ViewModels;

public class BookingViewModel
{
    public int RoomId { get; set; }
    public string? RoomName { get; set; }
    public decimal? PricePerNight { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Guests { get; set; }
}
