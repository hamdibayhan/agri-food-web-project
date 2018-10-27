using System;
using System.ComponentModel.DataAnnotations;
using AgriFood.Models;
using Microsoft.AspNetCore.Http;

namespace AgriFood.ViewModels
{
    public class ProductCreateEditViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public Boolean IsActive { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string FarmerId { get; set; }
        [ValidateImage]
        public IFormFile Image { get; set; }
    }
}
