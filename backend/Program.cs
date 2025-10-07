using TMKMiniApp.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using FluentValidation;
using TMKMiniApp.Validators;
using TMKMiniApp;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure form options for file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
    options.ValueLengthLimit = int.MaxValue;
    options.ValueCountLimit = int.MaxValue;
    options.KeyLengthLimit = int.MaxValue;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "TMK Mini App API", 
        Version = "v1",
        Description = "API для Telegram Mini App - цифровизация и автоматизация консультаций и заказов трубной продукции"
    });
});

// Add CORS with security restrictions
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5177", "http://localhost:5178", "http://localhost:8080", "https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    // Add a more permissive policy for Swagger
    options.AddPolicy("AllowSwagger", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.AddFixedWindowLimiter("ApiPolicy", options =>
    {
        options.PermitLimit = 50;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    });
});

// Register services with Dependency Injection
builder.Services.AddSingleton<IDiscountService, DiscountService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IDataSyncService, DataSyncService>();
builder.Services.AddSingleton<JsonDataService>();
builder.Services.AddScoped<TMKMiniApp.Validators.OrderRequestValidator>();

// Register Delta Updates Service
builder.Services.AddSingleton<IDynamicDeltaUpdatesService, DynamicDeltaUpdatesService>();

// Register Automated Data Sync Service
builder.Services.AddSingleton<IAutomatedDataSyncService, AutomatedDataSyncService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for demo purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TMK Mini App API v1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    c.DocumentTitle = "TMK Mini App API Documentation";
    c.DefaultModelsExpandDepth(-1); // Hide models section
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
    c.EnableValidator();
    c.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
    c.ConfigObject.AdditionalItems.Add("tryItOutEnabled", true);
});

// Add CORS for Swagger endpoints
app.UseCors("AllowSwagger");

// Add a simple redirect from root to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

// Security middleware - CORS must be before other middleware
app.UseCors("AllowSpecificOrigins");
app.UseRateLimiter();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseAuthorization();
app.MapControllers();

// Запуск сервиса динамических обновлений
var deltaUpdatesService = app.Services.GetRequiredService<IDynamicDeltaUpdatesService>();
await deltaUpdatesService.StartMonitoringAsync();

// Автоматизированный сервис синхронизации запускается автоматически как IHostedService

// Тест десериализации удален - используется API endpoint /api/jsondata/validate

app.Run();