namespace WebApi.Orders
{
    public enum EStatus
    {
        Created,
        StockReserved,
        StockUnavailable,
        PaymentProcessed,
        PaymentFailed,
        Shipped,
        Delivered,
        DeliveryFailed,
        Cancelled
    }
}
