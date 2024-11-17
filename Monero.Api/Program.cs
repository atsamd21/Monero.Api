using Microsoft.OpenApi.Models;
using Monero.Api.Data;
using Monero.Api.Middleware;
using Monero.Api.Models;
using Monero.Api.Services;
using Monero.Api.Singletons;
using Monero.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFrameworkSqlite().AddDbContext<MoneroApiContext>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("ApiKeyHeader", new OpenApiSecurityScheme()
    {
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "x-api-key authorization",
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKeyHeader" }
        },
        Array.Empty<string>()
    }
  });
});

builder.Services.AddMonero();
builder.Services.AddCors();

builder.Services.Configure<BigCommerceSettings>(builder.Configuration.GetRequiredSection("BigCommerceSettings"));

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBigCommerceService, BigCommerceService>();

builder.Services.AddHttpClient<IBigCommerceService, BigCommerceService>(x =>
{
    x.BaseAddress = new Uri(builder.Configuration.GetRequiredSection("BigCommerceEndpoint").Get<string>() ?? throw new Exception("BigCommerceEndpoint must be set."));
});

builder.Services.AddActivatedSingleton<PaymentSingleton>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMonero();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();
app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

app.MigrateAndSeedDb<MoneroApiContext>();

app.Run();

// For tests. Can be done from csproj too
public partial class Program { }
