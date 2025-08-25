namespace WebApi.Orders.Messages
{
    // Pedido
    public record CreateOrder
    {
        public Guid CartId { get; set; }
        public Guid CustomerId { get; init; }
        public decimal TotalPrice { get; init; }
    }

    // Estoque
    public record ReserveStock
    {
        public Guid OrderId { get; init; }
        public int Quantity { get; init; }
        public string ProductCode { get; init; } = string.Empty;
    }

    public record CancelStockReservation
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
    }

    // Pagamento
    public record ProcessPayment
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
    }

    public record RefundPayment
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public string Reason { get; init; } = string.Empty;
    }

    // Entrega
    public record ShipOrder
    {
        public Guid OrderId { get; init; }
        public string Address { get; init; } = string.Empty;
    }

    public record CancelDelivery
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
    }
}
