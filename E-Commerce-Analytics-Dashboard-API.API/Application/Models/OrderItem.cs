using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Analytics_Dashboard_API.API.Application.Models
{
    [Table("order_items")]
    public class OrderItem
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("order_id")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
    }
}
