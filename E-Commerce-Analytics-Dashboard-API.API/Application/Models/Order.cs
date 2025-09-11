using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Analytics_Dashboard_API.API.Application.Models
{
    [Table("orders")]
    public class Order
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("customer_id")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
