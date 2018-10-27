using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AgriFood.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FarmName { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
