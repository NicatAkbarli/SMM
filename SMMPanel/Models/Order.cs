using SMMPanel.Models;

public class Order
{
    public int Id { get; set; }

    public int ServiceId { get; set; }
    public ServiceModel Service { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Link { get; set; } = null!;
    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Pending"; // ✅ əlavə etdik
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
