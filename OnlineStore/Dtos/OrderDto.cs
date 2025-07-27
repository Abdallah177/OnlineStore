using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Dtos
{
    public class OrderDto
    {
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
