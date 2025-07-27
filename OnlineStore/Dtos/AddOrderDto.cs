using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Dtos
{
    public class AddOrderDto 
    {

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public List<int> ProductIds { get; set; } = new List<int>();

    }
}
