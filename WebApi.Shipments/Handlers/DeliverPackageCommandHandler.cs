using Library.MessagingContracts.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebApi.Shipments.Messages;

namespace WebApi.Shipments.Handlers
{
    public class DeliverPackageCommandHandler(AppDbContext appDbContext) : IConsumer<DeliverPackageCommand>
    {
        public async Task Consume(ConsumeContext<DeliverPackageCommand> context)
        {

            Models.Shipment? shipment = await appDbContext.Shipments.FirstOrDefaultAsync(i => i.OrderId.Equals(context.Message.OrderId));
            if (shipment is null)
            {
                throw new Exception($"Shipment with OrderId {context.Message.OrderId} not found.");
            }

            shipment.Status = Models.ShipmentStatus.Delivered;
            shipment.DeliveredAt = DateTime.UtcNow;
            await appDbContext.SaveChangesAsync();

            await context.Publish(new ShipmentDeliveredEvent
            {
                OrderId = context.Message.OrderId,
                DeliveredAt = DateTime.UtcNow,
            });
        }
    }
}
