
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Shipments.Messages;

namespace WebApi.Shipments
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

            builder.Services.AddDbContext<AppDbContext>(options =>
            {

                options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DbConnectionString"));
                options.EnableSensitiveDataLogging();
            });

            builder.Services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumers(typeof(Program).Assembly);

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

            app.MapGet("/shipments", async (AppDbContext appDbContext) =>
            {
                return Results.Ok(await appDbContext.Shipments.AsNoTracking().ToListAsync());
            });

            app.MapPost("/shipments/delivery/{orderId}", async ([FromRoute] Guid orderId, AppDbContext appDbContext, IBus bus) =>
            {
                DeliverPackageCommand order = new(orderId);
                await bus.Publish(order);

                return Results.Accepted();
            });


            app.MapPost("/", async (AppDbContext appDbContext, IBus bus) =>
            {
                //var order = new CreateOrder(Guid.NewGuid(), Guid.NewGuid(), new Random().Next(0, 1000));
                //await bus.Publish(order);

                return Results.Accepted();
            });

            app.MapControllers();

            await app.RunAsync(); // Changed to RunAsync to support async Main
        }
    }
}
