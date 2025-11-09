# WarehouseMS

In-memory Warehouse Management demo (console app) that demonstrates stock allocation for customer orders.

## What it is
- Lightweight demo app that models `Product`, `Location`, `SKU`, `Order`, `OrderLine` and `Allocation`.
- Implements a simple allocation engine that assigns SKU quantities to released orders based on priority and FIFO ordering.

## Features
- Allocate released orders from available SKUs.
- Skip SKUs stored in locked locations.
- Support partial allocations across multiple SKUs.
- Rollback allocations for orders that require complete delivery when insufficient stock exists.
- Simple console UI for viewing SKUs/orders, allocation, cancellation and SKU quantity corrections.

## Key files
- `Program.cs` — seeds data, initializes services and shows the console menu.
- `Application/Services/AllocationService.cs` — allocation algorithm (priority, FIFO, rollbacks).
- `Application/Services/SkuRepository.cs` & `OrderRepository.cs` — in-memory repositories.
- `Domain/Models/*` — domain model classes (`SKU`, `Location`, `Order`, `OrderLine`, `Allocation`, `Product`).

## How allocation works
1. The service selects released orders ordered by priority (High ? Normal ? Low) and FIFO for same priority.
2. For each order line, it consumes SKUs for the product (skipping locked locations) until the line is satisfied.
3. If `CompleteDeliveryRequired` is true and the order cannot be fully allocated, allocations for that order are rolled back.
4. The same SKU may be allocated to multiple orders as long as its available quantity permits.

## Requirements
- .NET 8 SDK

## Run locally
1. Open a terminal in the repository root (where `WarehouseMS.csproj` is located).
2. Build (optional): __dotnet build__
3. Run: __dotnet run__ or __dotnet run --project WarehouseMS.csproj__

The console menu supports:
- View SKUs
- View Orders
- Allocate Released Orders
- Cancel Order
- Correct SKU Quantity

## Seed/demo data
`Program.cs` seeds sample products, locations (one locked), SKUs and a few orders to demonstrate priority, FIFO and allocation behavior.

## Notes & limitations
- All data is in-memory; there is no persistence or database.
- No transaction manager / concurrency control — rollback is performed in-memory.
- Minimal error handling and logging (intended as a demo).
- To persist or add transactions, replace repositories with a DB-backed implementation (for example, EF Core) and add a unit-of-work.
