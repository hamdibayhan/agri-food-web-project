using System;

namespace AgriFood.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Boolean IsActive { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }

        public byte[] Image { get; set; }

        public string FarmerId { get; set; }
        public ApplicationUser Farmer { get; set; }
    }
}
