using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Sagas;

public class ProductOrderingSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    // Pedido
    public Guid OrderId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }

    // Estoque
    public DateTime? StockCheckedAt { get; set; }
    public Guid ProductId { get;  set; }
    public int Quantity { get;  set; }

    // Pagamento
    public decimal AmountPaid { get; set; }
    public DateTime? PaymentAt { get; set; }
    public DateTime? PaymentFailedAt { get; set; }

    // Entrega
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? DeliveryFailedAt { get; set; }
    public string Address { get;  set; }

    // Status
    public string? FailureReason { get; set; } = string.Empty;
    public bool IsCanceled { get; set; }
    public bool IsRefunded { get; set; }
    public bool IsCompleted { get; set; }

}
