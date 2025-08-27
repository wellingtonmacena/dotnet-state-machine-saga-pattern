
# ⚡ Saga Pattern with State Machine in .NET

This project demonstrates the implementation of the **Saga Pattern**, using a **State Machine** to orchestrate distributed transactions in a microservices environment.  
Communication between services is handled asynchronously using a **message broker** (**RabbitMQ**) and the **MassTransit** library.

---

## 🧩 What is the Saga Pattern?

The Saga Pattern is an architectural approach for managing **long-lived, business-critical transactions** in distributed systems where a single, atomic ACID transaction is not feasible.  

A saga is a sequence of **local, decoupled transactions**:
- Each service executes its own transaction.
- Services publish **events** or **commands** to trigger the next step.
- If a step fails, the saga triggers **compensating transactions** to undo previous operations, ensuring data consistency.

### 🔀 Implementation Styles

1. **Choreography**  
   - Decentralized: services react to each other’s events.  
   - Works well for small workflows, but grows complex at scale.  

2. **Orchestration**  
   - Centralized: a **Saga Orchestrator** manages the workflow.  
   - Easier to maintain, monitor, and handle failures.  
   - 👉 **This project uses orchestration with a state machine.**

---

## 🛒 Application Overview

The application simulates a typical **e-commerce order process**, a classic case of a business transaction spanning multiple domains.

### 🔧 Microservices

- **WebApi.Orders**  
  - Saga orchestrator  
  - Manages the state of `ProductOrderingSaga`  
  - Entry point for order creation  

- **WebApi.Inventory**  
  - Manages product stock  
  - Handles `ReserveStockCommand` → publishes `InventoryReservedEvent` or `InventoryOutOfStockEvent`  

- **WebApi.Payments**  
  - Processes payments  
  - Handles `ProcessPaymentCommand` → publishes `PaymentSucceededEvent` or `PaymentFailedEvent`  

- **WebApi.Shipments**  
  - Manages shipments  
  - Handles `ShipOrderCommand` → publishes `ShipmentDispatchedEvent` or `ShipmentDeliveredEvent`  

- **Library.MessagingContracts**  
  - Shared library with message contracts (commands & events)  
  - Ensures consistency in communication  

- **docker-compose.yaml**  
  - Infrastructure setup (PostgreSQL, RabbitMQ)  

---

## 🔄 Event Flow

Below is the orchestration flow for both a **successful order** and the **compensation scenario** when payment fails:

```mermaid
graph TD
    A[Order API: POST /orders] -->|Publishes CreateOrderCommand| B{MassTransit Bus}
    B -->|Consumes CreateOrderCommand| C[Orders API: CreateOrderCommandHandler]
    C -->|Persists Order, Publishes OrderCreatedEvent| D[Saga: ProductOrderingSaga]
    D -->|Publishes ReserveStockCommand| E[Inventory API]
    
    E -->|Inventory Available| F[Saga: ProductOrderingSaga]
    F -->|Publishes ProcessPaymentCommand| G[Payments API]
    
    G -->|Payment Succeeded| H[Saga: ProductOrderingSaga]
    H -->|Publishes ShipOrderCommand| I[Shipments API]
    
    I -->|Publishes ShipmentDispatchedEvent| J[Saga: ProductOrderingSaga]
    J -->|Publishes DeliverPackageCommand| K[Shipments API]
    K -->|Publishes ShipmentDeliveredEvent| L[Saga Completed ✅]

    %% Failures
    E -->|Out of Stock| M[Saga: Ends ❌ Stock Failure]
    G -->|Payment Failed| N[Saga: Compensation Trigger]
    N -->|Publishes ReturnStockCommand| O[Inventory API: Releases Stock]
    O -->|Publishes InventoryReleasedEvent| P[Saga Ends with Compensation ♻️]
````

---

## 🚀 Getting Started

### 1️⃣ Clone the Repository

```bash
git clone https://github.com/your-repo/dotnet-state-machine-saga-pattern.git
cd dotnet-state-machine-saga-pattern
```

### 2️⃣ Start Dependencies with Docker

The solution requires **RabbitMQ** (and optionally PostgreSQL).
Run the provided Docker setup:

```bash
docker-compose up -d
```

🔗 RabbitMQ Management Console: [http://localhost:15672](http://localhost:15672)
(default user: `guest`, password: `guest`)

---

## 🛠️ Build the Solution

From the root folder:

```bash
dotnet build SagaPattern.sln
```

---

## ▶️ Running the Microservices

Each microservice is an independent WebAPI project. Run them individually:

### Orders API

```bash
cd WebApi.Orders
dotnet run
```

👉 Swagger: [https://localhost:7005/swagger](https://localhost:7005/swagger)

---

### Inventory API

```bash
cd WebApi.Inventory
dotnet run
```

---

### Payments API

```bash
cd WebApi.Payments
dotnet run
```

---

### Shipments API

```bash
cd WebApi.Shipments
dotnet run
```

---

## 📚 References

* [MassTransit Documentation](https://masstransit.io/documentation/concepts)
* [Saga Pattern – Microservices.io](https://microservices.io/patterns/data/saga.html)
* [Microsoft – Saga Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/saga)

---

## 📬 Contact

* **Email:** [wellington.macena.23@gmail.com](mailto:wellington.macena.23@gmail.com)
* **LinkedIn:** [Wellington Macena](https://www.linkedin.com/in/wellingtonmacena/)

---