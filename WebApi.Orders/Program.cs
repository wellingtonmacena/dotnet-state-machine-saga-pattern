
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
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options => {

                
                options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DbConnectionString"));
                options.EnableSensitiveDataLogging();
            });

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumers(typeof(Program).Assembly);

                busConfigurator.AddSagaStateMachine<ProductOrderingSaga, ProductOrderingSagaData>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ExistingDbContext<AppDbContext>();
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires a RowVersion column
                        r.UsePostgres();
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

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });


            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                using IServiceScope scope = app.Services.CreateScope();
                await using AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
                await dbContext.Database.EnsureCreatedAsync();
            }

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(c =>
            {
                c.EnableTryItOutByDefault();
                c.DisplayRequestDuration();
            });


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/", async (AppDbContext appDbContext) =>
            {
                return Results.Ok(await appDbContext.Orders.AsNoTracking().ToListAsync());
            });

            app.MapPost("/", async (AppDbContext appDbContext, IBus bus) =>
            {
                var paymentMethods = Enum.GetValues<PaymentMethod>();
                var randomNumber = new Random().Next(0, paymentMethods.Length - 1);

              
                var order = new CreateOrder(Guid.Parse("3a1f2c44-5f6d-4e5e-9b3f-21a7e8d1c001"), 2, new Random().Next(0, 1000), paymentMethods[randomNumber]);
                await bus.Publish(order);

                return Results.Accepted();
            });

            app.MapControllers();

            await app.RunAsync(); // Changed to RunAsync to support async Main
        }
    }
}
