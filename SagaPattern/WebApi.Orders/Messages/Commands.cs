namespace WebApi.Orders.Messages
{
    // Pedido
    public record CreateOrder(Guid CartId, Guid CustomerId, decimal TotalPrice);

    // Estoque
    public record ReserveStock(Guid OrderId, Guid CartId);

    public record CancelStockReservation(Guid OrderId, string Reason = "");

    // Pagamento
    public record ProcessPayment(Guid OrderId, decimal Amount, string PaymentMethod = "");

    public record RefundPayment(Guid OrderId, decimal Amount, string Reason = "");

    // Entrega
    public record ShipOrder(Guid OrderId, string Address = "");

    public record CancelDelivery(Guid OrderId, string Reason = "");
}
