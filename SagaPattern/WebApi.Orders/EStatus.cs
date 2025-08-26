namespace WebApi.Orders
{
    public enum EStatus
    {
        Created,
        StockReserved,
        StockReservationFailed,
        PaymentProcessed,
        PaymentFailed,
        Shipped,
        Delivered,
        DeliveryFailed,
        Cancelled
    }
}
