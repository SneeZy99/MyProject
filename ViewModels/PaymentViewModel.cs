namespace kkkk11.ViewModels;

public class PaymentViewModel
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string? RoomName { get; set; }
    public string? CheckIn { get; set; }
    public string? CheckOut { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
}