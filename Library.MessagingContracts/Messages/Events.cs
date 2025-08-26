namespace Library.MessagingContracts.Messages
{

    // Pedido
    public record OrderCreated
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    // Estoque

    public record CheckProductsAvailableEventReceived
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    public record ProductsAvailableChecked
    {
        public Guid OrderId { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    public record ProductsUnavailableChecked
    {
        public Guid OrderId { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    // Pagamento
    public record PaymentProcessedSuccessful
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public DateTime PaidAt { get; init; }
    }

    public record PaymentProcessedFailed
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime FailedAt { get; init; }
    }

    // Entrega
    public record OrderShippedEvent
    {
        public Guid OrderId { get; init; }
        public DateTime ShippedAt { get; init; }
    }

    public record OrderDeliveredEvent
    {
        public Guid OrderId { get; init; }
        public DateTime DeliveredAt { get; init; }
    }

    public record DeliveryFailedEvent
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime FailedAt { get; init; }
    }
}

