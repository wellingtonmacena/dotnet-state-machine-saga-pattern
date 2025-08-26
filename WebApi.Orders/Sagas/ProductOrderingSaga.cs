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
    public Event<OrderCreated> OrderCreatedEvent { get; set; }

    public Event<ProductsAvailableChecked> ProductsAvailableChecked { get; set; }
    public Event<ProductsUnavailableChecked> ProductsUnavailableChecked { get; set; }

    public Event<PaymentSuccessfulProcessed> PaymentSuccessfulProcessed { get; set; }
    public Event<PaymentFailedProcessed> PaymentFailedProcessed { get; set; }

    public Event<OrderShippedEvent> OrderShippedEvent { get; set; }
    public Event<OrderDeliveredEvent> OrderDeliveredEvent { get; set; }
    public Event<DeliveryFailedEvent> DeliveryFailedEvent { get; set; }

    public ProductOrderingSaga()
    {
        InstanceState(x => x.CurrentState);

        // correlaciona pelo OrderId
        Event(() => OrderCreatedEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => ProductsAvailableChecked, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => ProductsUnavailableChecked, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentSuccessfulProcessed, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentFailedProcessed, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderShippedEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderDeliveredEvent, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => DeliveryFailedEvent, e => e.CorrelateById(m => m.Message.OrderId));

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
                })
                .TransitionTo(OrderCreated)
                .Publish(ctx => new ReserveStock(
                    ctx.Message.OrderId,
                    ctx.Message.ProductId,
                   ctx.Message.Quantity
                ))
        );

        // Verificação de estoque
        During(OrderCreated,
        When(ProductsAvailableChecked)
            .Then(ctx =>
            {
                ctx.Saga.StockCheckedAt = ctx.Message.CheckedAt;

            })
            .TransitionTo(ProductInStock)
            .Publish(ctx => new ProcessPayment(
                    ctx.Message.OrderId,
                    ctx.Message.TotalPrice,
                    ctx.Saga.PaymentMethod
                )),

        When(ProductsUnavailableChecked)
            .Then(ctx =>
            {
                ctx.Saga.StockCheckedAt = ctx.Message.CheckedAt;
                ctx.Saga.IsCanceled = true;
            })
            .TransitionTo(ProductOutOfStock)
            .Publish(ctx => new CancelStockReservation(ctx.Saga.OrderId))
    );

        // Pagamento
        During(ProductInStock,
            When(PaymentSuccessfulProcessed)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentAt = ctx.Message.PaidAt;
                    ctx.Saga.AmountPaid = ctx.Message.Amount;
                })
                .TransitionTo(PaymentSuccessful),

            When(PaymentFailedProcessed)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentFailedAt = ctx.Message.FailedAt;
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                    ctx.Saga.IsRefunded = true;
                })
                .TransitionTo(PaymentFailed)
                .Publish(ctx => new ReturnStock(ctx.Saga.OrderId, ctx.Saga.ProductId, ctx.Saga.Quantity))
               // .Publish(ctx => new RefundPayment(ctx.Saga.OrderId, ctx.Saga.Quantity , ctx.Saga.FailureReason))
        );

        //// Envio
        //During(PaymentSuccessful,
        //    When(OrderShippedEvent)
        //        .Then(ctx => ctx.Saga.ShippedAt = ctx.Message.ShippedAt)
        //        .TransitionTo(OrderShipped)
        //);

        //// Entrega
        //During(OrderShipped,
        //    When(OrderDeliveredEvent)
        //        .Then(ctx =>
        //        {
        //            ctx.Saga.DeliveredAt = ctx.Message.DeliveredAt;
        //            ctx.Saga.IsCompleted = true;
        //        })
        //        .TransitionTo(OrderDelivered)
        //        .Finalize(),

        //    When(DeliveryFailedEvent)
        //        .Then(ctx =>
        //        {
        //            ctx.Saga.DeliveryFailedAt = ctx.Message.FailedAt;
        //            ctx.Saga.FailureReason = ctx.Message.Reason;
        //            ctx.Saga.IsCanceled = true;
        //        })
        //        .TransitionTo(DeliveryFailed)
        //        .Finalize()
        //);

        SetCompletedWhenFinalized();
    }
}
