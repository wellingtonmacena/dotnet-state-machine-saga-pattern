using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Inventory.Handlers
{
    public class ProductsReturnedHandler(AppDbContext appDbContext) : IConsumer<ProductsReturned>
    {
        public async Task Consume(ConsumeContext<ProductsReturned> context)
        {
            Models.StockItem? stockItem = appDbContext.StockItems.FirstOrDefault(p => p.ProductId == context.Message.ProductId);

            if (stockItem != null) // Ensure stockItem is not null before dereferencing
            {
                stockItem.Quantity += context.Message.Quantity;
                appDbContext.StockItems.Update(stockItem);
                await appDbContext.SaveChangesAsync();
            }            
        }
    }
}
