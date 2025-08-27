namespace WebApi.Shipments.Messages
{
    public record DeliverPackageCommand(Guid OrderId)
    {
    }


}
