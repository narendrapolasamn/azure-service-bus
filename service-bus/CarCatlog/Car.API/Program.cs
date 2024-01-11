
using Car.API.Core.DependencyInjection;
using Car.Infrastructure.Filters;
using Car.Infrastructure.Repositories;
using CarsIsland.Catalog.API.Core.DependencyInjection;
using EventLog;
using FluentValidation.AspNetCore;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAppConfiguration(configuration);
builder.Services.AddDataService();
builder.Services.AddIntegrationServices();
//builder.Services.AddSwagger();
builder.Services.AddModelValidators();
builder.Services.AddControllers(configure =>
{
    configure.Filters.Add(typeof(HttpGlobalExceptionFilter));
}).AddFluentValidation();

var app = builder.Build();
using var serviceScope = app.Services.CreateScope();
DataServiceCollectionExtensions.Migrate<EventLogContext>(serviceScope.ServiceProvider);
DataServiceCollectionExtensions.Migrate<CarCatalogDbContext>(serviceScope.ServiceProvider);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseDeveloperExceptionPage();
}

app.UseSwaggerServices();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();