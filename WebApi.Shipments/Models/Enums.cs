namespace WebApi.Shipments.Models
{
    public enum ShipmentStatus
    {
        Pending = 0,   // Preparando
        Shipped = 1,   // Enviado
        Delivered = 2, // Entregue
        Failed = 3     // Falha na entrega
    }
}
