using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMMPanel.Models
{
    public class Request
    {
        public record SendCodeRequest(string Email, string Username, string Password);
        public record VerifyCodeRequest(string Email, string Code);

    }
}