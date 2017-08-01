using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class ImageDTO
    {
        public byte[] Slika { get; set; }
        public string Ime { get; set; }
        public DateTime UploadTimeStamp { get; set; }
        public bool Status { get; set; }
    }
}