using Library.MessagingContracts.Messages;

namespace WebApi.Orders
{
    public class Order: Entity
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public EStatus Status { get; set; }
    }
}
