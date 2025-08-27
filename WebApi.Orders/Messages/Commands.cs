using Library.MessagingContracts.Messages;

namespace WebApi.Orders.Messages
{
    // Pedido
    public record CreateOrderCommand(Guid ProductId, int Quantity, PaymentMethod PaymentMethod, string Address);

    // Estoque
    public record ReserveStockCommand(Guid OrderId, Guid ProductId, int Quantity);
    public record ReturnStockCommand(Guid OrderId, Guid ProductId, int Quantity);

    public record CancelStockReservationCommand(Guid OrderId, string Reason = "");

    // Pagamento
    public record ProcessPaymentCommand(Guid OrderId, decimal Amount, PaymentMethod PaymentMethod);

    public record RefundPaymentCommand(Guid OrderId, decimal Amount, string Reason = "");

    // Entrega
    public record ShipOrderCommand(Guid OrderId, string Address = "");

    public record CancelDeliveryCommand(Guid OrderId, string Reason = "");
}
