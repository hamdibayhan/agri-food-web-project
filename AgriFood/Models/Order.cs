using System;
namespace AgriFood.Models
{
    public class Order
    {
        public int ID { get; set; }
        public string Address { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }

        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
