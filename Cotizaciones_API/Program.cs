using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces.Cliente;
using Cotizaciones_API.Interfaces.Cotizacion;
using Cotizaciones_API.Interfaces.TipoSeguro;
using Cotizaciones_API.Interfaces.Moneda;
using Cotizaciones_API.Mapping; // MappingProfile
using Cotizaciones_API.Repositories.Cliente;
using Cotizaciones_API.Repositories.Cotizacion;
using Cotizaciones_API.Repositories.TipoSeguro;
using Cotizaciones_API.Repositories.Moneda;
using Cotizaciones_API.Services.Cliente;
using Cotizaciones_API.Services.Cotizacion;



//using Cotizaciones_API.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configuración DB y Dapper
builder.Services.AddSingleton<DapperContext>();

// Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICotizacionRepository, CotizacionRepository>();
builder.Services.AddScoped<ITipoSeguroRepository, TipoSeguroRepository>();
builder.Services.AddScoped<IMonedaRepository, MonedaRepository>();

// Servicios
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICotizacionService, CotizacionService>();

// Utilidades
//builder.Services.AddSingleton<IEmailSender, EmailSender>();
//builder.Services.AddScoped<IExcelExporter, ExcelExporter>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handler middleware (simple)
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(error, "Unhandled exception");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { Message = "Ocurrió un error en el servidor." });
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
