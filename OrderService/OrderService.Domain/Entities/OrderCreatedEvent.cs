namespace OrderService.Domain.Entities
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
