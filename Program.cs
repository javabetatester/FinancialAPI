using FinancialAPI.Data;
using FinancialAPI.Models;
using FinancialAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;




var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5127");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Financial API",
        Version = "v1",
        Description = @"
 **Financial API - Demonstração .NET 8**

Uma API REST completa para cotações de ações e gestão de watchlist.

**Funcionalidades:**
-  Cotações em tempo real (dados mockados)
-  Histórico de preços
-  Watchlist personalizada
-  Análise técnica avançada
-  Persistência com SQLite
-  Cache em memória

**Tecnologias:**
- .NET 8 Web API
- Entity Framework Core
- SQLite Database
- Memory Caching",
    });


    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<YahooFinanceService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "FinancialAPI/1.0");
});

builder.Services.AddScoped<IStockService, StockService>();


builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial API V1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Financial API - Documentation";
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/health", () => new {
    Status = "Healthy",
    Timestamp = DateTime.Now,
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName
}).WithTags("Health");

app.MapGet("/info", () => new {
    ApiName = "Financial API",
    Version = "1.0.0",
    Description = "API REST para cotações de ações",
    Documentation = "/swagger",
    Endpoints = new
    {
        Stocks = "/api/stocks/{symbol}",
        History = "/api/stocks/{symbol}/history",
        Watchlist = "/api/watchlist",
        Analysis = "/api/analysis/{symbol}"
    }
}).WithTags("Info");

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        context.Database.EnsureCreated();

        if (!context.WatchlistItems.Any())
        {
            context.WatchlistItems.AddRange(
                new WatchlistItem { Symbol = "AAPL", Name = "Apple Inc.", AddedAt = DateTime.Now.AddDays(-5) },
                new WatchlistItem { Symbol = "MSFT", Name = "Microsoft Corporation", AddedAt = DateTime.Now.AddDays(-3) },
                new WatchlistItem { Symbol = "GOOGL", Name = "Alphabet Inc.", AddedAt = DateTime.Now.AddDays(-1) }
            );
            context.SaveChanges();
            Console.WriteLine("Database created with sample data");
        }
        else
        {
            Console.WriteLine("Database ready");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error setting up database: {ex.Message}");
    }
}

Console.WriteLine("Financial API is running!");
Console.WriteLine($"Swagger Documentation: http://localhost:{app.Configuration["ASPNETCORE_URLS"]?.Split(':').Last() ?? "5127"}");
Console.WriteLine("Test endpoints: /health, /info, /api/stocks/AAPL");

app.Run();