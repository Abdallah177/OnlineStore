namespace OnlineStore.Dtos
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductsInOrderDto> Products { get; set; } = new();
    }
}
