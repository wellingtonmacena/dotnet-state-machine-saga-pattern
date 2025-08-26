using Library.MessagingContracts.Messages;

namespace WebApi.Orders.Messages
{
    // Pedido
    public record CreateOrder(Guid ProductId, int Quantity, PaymentMethod PaymentMethod);

    // Estoque
    public record ReserveStock(Guid OrderId, Guid ProductId, int Quantity);
    public record ReturnStock(Guid OrderId, Guid ProductId, int Quantity);

    public record CancelStockReservation(Guid OrderId, string Reason = "");

    // Pagamento
    public record ProcessPayment(Guid OrderId, decimal Amount, PaymentMethod PaymentMethod);

    public record RefundPayment(Guid OrderId, decimal Amount, string Reason = "");

    // Entrega
    public record ShipOrder(Guid OrderId, string Address = "");

    public record CancelDelivery(Guid OrderId, string Reason = "");
}
