using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Shipments.Handlers
{
    public class ShipmentCreatedEventHandler(AppDbContext appDbContext) : IConsumer<ShipmentCreatedEvent>
    {
        public async Task Consume(ConsumeContext<ShipmentCreatedEvent> context)
        {
           var shipment = new Models.Shipment
           {
               OrderId = context.Message.OrderId,
               ShippedAt = DateTime.UtcNow,
               Status = Models.ShipmentStatus.Pending,
               Carrier = "Default Carrier",
               DestinationAddress = context.Message.Address,
               TrackingNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),           
           };

            _ = appDbContext.Shipments.Add(shipment);
            _ = appDbContext.SaveChanges();
            

            await context.Publish(new ShipmentDispatchedEvent
            {
                OrderId = shipment.OrderId,
                Address = shipment.DestinationAddress,
                ShippedAt = shipment.ShippedAt,
            });
        }
    }
}
