using Microsoft.EntityFrameworkCore;
using MySpot.Api.Endpoints;
using MySpot.App;
using MySpot.App.Services;
using MySpot.Core;
using MySpot.Core.ValueObjects;
using MySpot.Infrastructure;
using MySpot.Infrastructure.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddCore();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
// app.UseHttpsRedirection();
app.MapWeatherForecastApi();
app.MapReservationsV1();

app.Run();
