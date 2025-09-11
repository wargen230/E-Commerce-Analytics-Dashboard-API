using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Analytics_Dashboard_API.API.Application.Models
{
    [Table("customers")]
    public class Customer
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("registration_date")]
        public DateTime RegistrationDate { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
