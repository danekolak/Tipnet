using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class UsernameAndEmailDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool AccDisabled { get; set; }
    }
}