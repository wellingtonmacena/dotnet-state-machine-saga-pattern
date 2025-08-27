using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Sagas;

public class ProductOrderingSaga : MassTransitStateMachine<ProductOrderingSagaData>
{
    // States
    public State OrderCreated { get; set; }

    public State ProductInStock { get; set; }
    public State ProductOutOfStock { get; set; }

    public State PaymentSuccessful { get; set; }
    public State PaymentFailed { get; set; }

    public State OrderShipped { get; set; }
    public State OrderDelivered { get; set; }
    public State DeliveryFailed { get; set; }

    // Events
    public Event<OrderCreatedEvent> OrderCreatedEvent { get; set; }

    public Event<InventoryReservedEvent> InventoryReservedEvent { get; set; }
    public Event<InventoryOutOfStockEvent> InventoryOutOfStockEvent { get; set; }

    public Event<PaymentSucceededEvent> PaymentSucceededEvent { get; set; }
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }

    public Event<ShipmentDispatchedEvent> ShipmentDispatchedEvent { get; set; }
    public Event<ShipmentDeliveredEvent> ShipmentDeliveredEvent { get; set; }
    public Event<ShipmentFailedEvent> ShipmentFailedEvent { get; set; }

    public ProductOrderingSaga()
    {
        InstanceState(x => x.CurrentState);

        // correlaciona pelo OrderId
        Event(() => OrderCreatedEvent, e => e.CorrelateById(m => m.Message.OrderId));

        Event(() => InventoryReservedEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => InventoryOutOfStockEvent, e => e.CorrelateById(m => m.Message.OrderId));

        Event(() => PaymentSucceededEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentFailedEvent, e => e.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDispatchedEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => ShipmentDeliveredEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => ShipmentFailedEvent, e => e.CorrelateById(m => m.Message.OrderId));

        // Pedido criado → vai para estado OrderCreated
        Initially(
            When(OrderCreatedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.CreatedAt = ctx.Message.CreatedAt;
                    ctx.Saga.ProductId = ctx.Message.ProductId;
                    ctx.Saga.Quantity = ctx.Message.Quantity;
                    ctx.Saga.PaymentMethod = ctx.Message.PaymentMethod;
                    ctx.Saga.Address = ctx.Message.Address;
                })
                .TransitionTo(OrderCreated)
                .Publish(ctx => new ReserveStockCommand(
                    ctx.Message.OrderId,
                    ctx.Message.ProductId,
                   ctx.Message.Quantity
                ))
        );

        // Verificação de estoque
        During(OrderCreated,
        When(InventoryReservedEvent)
            .Then(ctx =>
            {
                ctx.Saga.StockCheckedAt = ctx.Message.CheckedAt;
            })
            .TransitionTo(ProductInStock)
            .Publish(ctx => new ProcessPaymentCommand(
                    ctx.Message.OrderId,
                    ctx.Message.TotalPrice,
                    ctx.Saga.PaymentMethod
                ))
            ,

        When(InventoryOutOfStockEvent)
            .Then(ctx =>
            {
                ctx.Saga.StockCheckedAt = ctx.Message.CheckedAt;
                ctx.Saga.IsCanceled = true;
            })
            .TransitionTo(ProductOutOfStock)
            .Publish(ctx => new UpdateOrderCommand(ctx.Message.OrderId, EStatus.StockUnavailable))
    );

        // Pagamento
        During(ProductInStock,
            When(PaymentSucceededEvent)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentAt = ctx.Message.PaidAt;
                    ctx.Saga.AmountPaid = ctx.Message.Amount;
                })
                .TransitionTo(PaymentSuccessful)
                .Publish(ctx => new ShipOrderCommand(
                    ctx.Message.OrderId,
                    ctx.Saga.Address
                )),

            When(PaymentFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentFailedAt = ctx.Message.FailedAt;
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                    ctx.Saga.IsRefunded = true;
                })
                .TransitionTo(PaymentFailed)
                .Publish(ctx => new ReturnStockCommand(ctx.Saga.OrderId, ctx.Saga.ProductId, ctx.Saga.Quantity, ctx.Message.Reason))
        // .Publish(ctx => new RefundPayment(ctx.Saga.OrderId, ctx.Saga.Quantity , ctx.Saga.FailureReason))
        );

        // Envio
        During(PaymentSuccessful,
            When(ShipmentDispatchedEvent)
                .Then(ctx => ctx.Saga.ShippedAt = ctx.Message.ShippedAt)
                .TransitionTo(OrderShipped)
        );

        // Entrega
        During(OrderShipped,
            When(ShipmentDeliveredEvent)
                .Then(ctx =>
                {
                    ctx.Saga.DeliveredAt = ctx.Message.DeliveredAt;
                    ctx.Saga.IsCompleted = true;
                })
                .TransitionTo(OrderDelivered),

            When(ShipmentFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.DeliveryFailedAt = ctx.Message.FailedAt;
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                    ctx.Saga.IsCanceled = true;
                })
                .TransitionTo(DeliveryFailed)
        );

        SetCompletedWhenFinalized();
    }
}
