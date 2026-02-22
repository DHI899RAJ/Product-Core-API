using ProductManagementAPI.Data;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Services;
using ProductManagementAPI.Services.DependencyInjection;
using ProductManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog for structured logging across the application
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog as the logging provider
    builder.Host.UseSerilog();

    // Add Database Context
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Add services to the container
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Product Management API",
            Version = "v1",
            Description = "A .NET 8 Web API for managing products"
        });
    });

    // Register AutoMapper
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Register repositories and services
    builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
    builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
    builder.Services.AddScoped<IRepository<Order>, Repository<Order>>();
    builder.Services.AddScoped<IRepository<OrderItem>, Repository<OrderItem>>();
    builder.Services.AddScoped<IRepository<Delivery>, Repository<Delivery>>();
    builder.Services.AddScoped<IRepository<Inventory>, Repository<Inventory>>();
    builder.Services.AddScoped<IRepository<Payment>, Repository<Payment>>();
    builder.Services.AddScoped<IRepository<RequestLog>, Repository<RequestLog>>();
    
    // Register domain services
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IDeliveryService, DeliveryService>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddSingleton<IRequestLoggingService, RequestLoggingService>();

    var app = builder.Build();

    // Add exception handling middleware (must be first in pipeline)
    app.UseMiddleware<ProductManagementAPI.Middleware.ExceptionHandlingMiddleware>();

    // Add request logging middleware (logs requests using Serilog and repository pattern)
    app.UseMiddleware<ProductManagementAPI.Middleware.RequestLoggingMiddleware>();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.MapControllers();

    Log.Information("Starting Product Management API");
    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
