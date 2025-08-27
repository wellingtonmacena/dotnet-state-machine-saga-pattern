
# Saga Pattern with State Machine in .NET

This project demonstrates the implementation of the **Saga pattern**, using a **State Machine** to orchestrate distributed transactions in a microservices environment. Communication between services is handled asynchronously using a **message broker** (RabbitMQ) and the **MassTransit** library.

---

## The Saga Pattern

The Saga pattern is an architectural approach to managing **long-lived, business-critical transactions** in distributed systems where a single, atomic ACID transaction is not feasible. A saga is a sequence of **local, decoupled transactions**, where each service performs its operation and then publishes a message or event to trigger the next step. If a step fails, the saga executes **compensating transactions** to undo the changes made by previous successful steps, ensuring data consistency across services.

### Implementation Approaches

1. **Choreography**
   - Decentralized approach
   - Each service publishes events consumed by others, triggering local transactions
   - No central coordinator
   - Simpler for smaller workflows, harder to manage at scale

2. **Orchestration**
   - Centralized approach
   - A **Saga Orchestrator** manages the workflow
   - Orchestrator sends commands and reacts to events to determine next steps
   - Easier to monitor, maintain, and handle failures  
   - This project uses **orchestration with a state machine**

---

## Application and Project Descriptions

The application simulates a typical **e-commerce order process**, a classic example of a business transaction that spans multiple domains.

### Microservices

- **WebApi.Orders**
  - Acts as the **saga orchestrator**
  - Manages the state of `ProductOrderingSaga`
  - Entry point for creating an order
  - Initiates saga and reacts to events from other services

- **WebApi.Inventory**
  - Manages **product stock**
  - Listens for `ReserveStockCommand`
  - Publishes `InventoryReservedEvent` or `InventoryOutOfStockEvent`

- **WebApi.Payments**
  - Handles **payment processing**
  - Receives `ProcessPaymentCommand`
  - Publishes `PaymentSucceededEvent` or `PaymentFailedEvent`

- **WebApi.Shipments**
  - Manages **shipment process**
  - Receives `ShipOrderCommand`
  - Publishes `ShipmentDispatchedEvent` or `ShipmentDeliveredEvent`

- **Library.MessagingContracts**
  - Shared library defining message contracts (commands and events)
  - Ensures consistent inter-service communication

- **docker-compose.yaml**
  - Defines infrastructure containers
  - Includes PostgreSQL and RabbitMQ

---

## Flowchart of Events and Their Flows

The orchestration flow for a **successful order** and **compensation** in case of payment failure:

```mermaid
graph TD
    A[Order API: POST /orders] -->|Publishes CreateOrderCommand| B{MassTransit Bus}
    B -->|Consumes CreateOrderCommand| C[Orders API: CreateOrderCommandHandler]
    C -->|Persists Order, Publishes OrderCreatedEvent| D[Saga: ProductOrderingSaga]
    D -->|State: OrderCreated, Publishes ReserveStockCommand| E[Inventory API: InventoryCheckPerformedEventHandler]
    
    E -->|Inventory Available, Publishes InventoryReservedEvent| F[Saga: ProductOrderingSaga]
    F -->|State: ProductInStock, Publishes ProcessPaymentCommand| G[Payments API: PaymentInitiatedEventHandler]
    
    G -->|Payment Succeeded, Publishes PaymentSucceededEvent| H[Saga: ProductOrderingSaga]
    H -->|State: PaymentSuccessful, Publishes ShipOrderCommand| I[Shipments API: ShipmentCreatedEventHandler]
    
    I -->|Publishes ShipmentDispatchedEvent| J[Saga: ProductOrderingSaga]
    J -->|State: OrderShipped, Publishes DeliverPackageCommand| K[Shipments API: DeliverPackageCommandHandler]
    
    K -->|Publishes ShipmentDeliveredEvent| L[Saga: ProductOrderingSaga]
    L -->|State: OrderDelivered| Z[Saga Completed Successfully]

    E -->|Inventory Out of Stock, Publishes InventoryOutOfStockEvent| M[Saga: ProductOrderingSaga]
    M -->|State: ProductOutOfStock, Updates Order Status| N[Saga Ends with Stock Failure]

    G -->|Payment Failed, Publishes PaymentFailedEvent| O[Saga: ProductOrderingSaga]
    O -->|State: PaymentFailed, Publishes ReturnStockCommand| P[Inventory API: InventoryReleasedEventHandler]
    P -->|Releases Stock, Publishes InventoryReleasedEvent| Q[Saga Ends with Compensation]
````

---

## References

* **MassTransit Documentation**
  [https://masstransit.io/documentation/concepts](https://masstransit.io/documentation/concepts)

* **Saga Pattern on Microservices.io**
  [https://microservices.io/patterns/data/saga.html](https://microservices.io/patterns/data/saga.html)

* **Microsoft Azure Architecture Center**
  [https://learn.microsoft.com/en-us/azure/architecture/patterns/saga](https://learn.microsoft.com/en-us/azure/architecture/patterns/saga)

---

## Contact

* **Email:** [wellington.macena.23@gmail.com](mailto:wellington.macena.23@gmail.com)
* **LinkedIn:** [Wellington Macena](https://www.linkedin.com/in/wellingtonmacena/)
