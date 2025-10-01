using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMMPanel.Models
{
   public class ServiceModel
{
    public int Id { get; set; }
    public string Platform { get; set; } = null!; // Məs: TikTok, Instagram
    public string ServiceName { get; set; } = null!; // Məs: "Takipçi artırma"
    public decimal PricePer1000 { get; set; } // 1000 xidmət üçün qiymət
}

}