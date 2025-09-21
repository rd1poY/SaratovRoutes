using System;
using System.Collections.Generic;
using System.Text;

namespace SaratovRoutes.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string time { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string City { get; set; }
        public string ImgRoutes { get; set; }
        public string ImgRoute { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
    }
}
