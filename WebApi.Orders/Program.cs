
using Bogus;
using Library.MessagingContracts.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebApi.Orders.Messages;
using WebApi.Orders.Sagas;

namespace WebApi.Orders
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            _ = builder.Services.AddOpenApi();
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            _ = builder.Services.AddDbContext<AppDbContext>(options =>
            {


                _ = options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DbConnectionString"));
                _ = options.EnableSensitiveDataLogging();
            });

            _ = builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumers(typeof(Program).Assembly);

                _ = busConfigurator.AddSagaStateMachine<ProductOrderingSaga, ProductOrderingSagaData>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ExistingDbContext<AppDbContext>();

                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires a RowVersion column
                        _ = r.UsePostgres();
                    });

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    string? host = builder.Configuration["RabbitMQ:Host"];
                    string? user = builder.Configuration["RabbitMQ:Username"];
                    string? password = builder.Configuration["RabbitMQ:Password"];

                    cfg.Host(host, "/", h =>
                    {
                        h.Username(user);
                        h.Password(password);
                    });

                    cfg.UseInMemoryOutbox(context);

                    cfg.ConfigureEndpoints(context);
                });
            });

            _ = builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            _ = builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            _ = builder.Services.AddHttpClient("InventoryClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7016/");
            });

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.MapOpenApi();
                using IServiceScope scope = app.Services.CreateScope();
                await using AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
                _ = await dbContext.Database.EnsureCreatedAsync();
            }

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(c =>
            {
                c.EnableTryItOutByDefault();
                c.DisplayRequestDuration();
            });


            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.MapGet("/", async (AppDbContext appDbContext) =>
            {
                return Results.Ok(await appDbContext.Orders.AsNoTracking().ToListAsync());
            });

            _ = app.MapPost("/", async (AppDbContext appDbContext, IBus bus) =>
            {
                PaymentMethod[] paymentMethods = Enum.GetValues<PaymentMethod>();
                int randomNumber = new Random().Next(0, paymentMethods.Length - 1);
                var faker = new Faker("pt_BR");

                CreateOrderCommand order = new(Guid.Parse("3a1f2c44-5f6d-4e5e-9b3f-21a7e8d1c001"), 2, paymentMethods[randomNumber], faker.Address.FullAddress());
                await bus.Publish(order);

                return Results.Accepted();
            });

            _ = app.MapControllers();

            await app.RunAsync(); // Changed to RunAsync to support async Main
        }
    }
}
