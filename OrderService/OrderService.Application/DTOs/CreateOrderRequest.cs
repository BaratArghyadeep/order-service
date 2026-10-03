namespace OrderService.Application.DTOs
{
    public class CreateOrderRequest
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
