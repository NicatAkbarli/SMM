using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMMPanel.Data;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }


   [HttpPost("create")]
public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var user = await _context.Users.FindAsync(userId);
    if (user == null) return Unauthorized();

    var service = await _context.Services.FindAsync(request.ServiceId);
    if (service == null)
        return NotFound("Xidmət tapılmadı");

    var totalPrice = (request.Quantity / 1000m) * service.PricePer1000;

    if (user.Balance < totalPrice)
        return BadRequest(new { error = "Balans kifayət etmir" });

    // balansdan çıx
    user.Balance -= totalPrice;

    var order = new Order
    {
        UserId = userId,
        ServiceId = request.ServiceId,
        Link = request.Link,
        Quantity = request.Quantity,
        TotalPrice = totalPrice
    };

    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Sifariş yaradıldı", balance = user.Balance, order });
}



    [HttpGet("my-orders")]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var orders = await _context.Orders
            .Include(o => o.Service)
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return Ok(orders);
    }
}
