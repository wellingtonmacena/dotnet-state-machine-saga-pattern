namespace WebApi.Shipments.Models
{
    public class Shipment : Entity
    {
        public Guid OrderId { get; set; }

        public string Carrier { get; set; } = string.Empty;  // Ex: FedEx, DHL, Correios
        public string TrackingNumber { get; set; } = string.Empty; // Código de rastreamento
        public string DestinationAddress { get; set; } = string.Empty;

        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

        public DateTime ShippedAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public DateTime FailedAt { get; set; }
    }


}
