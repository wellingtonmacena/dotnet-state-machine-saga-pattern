namespace Library.MessagingContracts.Messages
{

    // Pedido
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal TotalPrice { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
        public string Address { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    // Estoque

    public record InventoryCheckPerformedEvent
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    public record InventoryReservedEvent
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal TotalPrice { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    public record InventoryOutOfStockEvent
    {
        public Guid OrderId { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    public record InventoryReleasedEvent
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public DateTime CheckedAt { get; init; }
    }

    // Pagamento

    public record PaymentInitiatedEvent
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public DateTime PaidAt { get; init; }
        public  PaymentMethod PaymentMethod { get; init; }
    }
    public record PaymentSucceededEvent
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public DateTime PaidAt { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
    }

    public record PaymentFailedEvent
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime FailedAt { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
    }

    public record PaymentRefundedEvent
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime FailedAt { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
    }

    // Entrega

    public record ShipmentCreatedEvent
    {
        public Guid OrderId { get; init; }
        public string Address { get; init; } = string.Empty;
        public DateTime ShippedAt { get; init; }
    }

    public record ShipmentDispatchedEvent
    {
        public Guid OrderId { get; init; }
        public string Address { get; init; } = string.Empty;
        public DateTime ShippedAt { get; init; }
    }

    public record ShipmentDeliveredEvent
    {
        public Guid OrderId { get; init; }
        public string Address { get; init; } = string.Empty;
        public DateTime DeliveredAt { get; init; }
    }

    public record ShipmentFailedEvent
    {
        public Guid OrderId { get; init; }
        public string Address { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
        public DateTime FailedAt { get; init; }
    }
}

