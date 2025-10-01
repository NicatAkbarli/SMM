using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMMPanel.Dtos
{
 public class ServiceDto
{
    public string Platform { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public decimal PricePer1000 { get; set; }
}

}