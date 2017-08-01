using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class PasswordResetDTO
    {
        public DateTime PrTimeStamp { get; set; }
        public string Username { get; set; }
    }
}