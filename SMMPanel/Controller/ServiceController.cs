using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMMPanel.Data;
using SMMPanel.Models;
using Microsoft.AspNetCore.Authorization;

namespace SMMPanel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        // Bütün xidmətləri gətir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _context.Services.ToListAsync();
            return Ok(services);
        }

        // Yeni xidmət əlavə et (Admin)
      
        [HttpPost]
        public async Task<IActionResult> Add(ServiceModel model)
        {
            _context.Services.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }
    }
}
