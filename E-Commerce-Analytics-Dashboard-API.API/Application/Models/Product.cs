using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Analytics_Dashboard_API.API.Application.Models
{
    [Table("products")]
    public class Product
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
