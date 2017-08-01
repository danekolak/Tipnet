using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class SaltAndPassDTO
    {
        public string Salt { get; set; }
        public string Password { get; set; }
        public int PogresanPass { get; set; }
        public DateTime VrijemeBlokade { get; set; }
    }
}