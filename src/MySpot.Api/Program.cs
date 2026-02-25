using MySpot.Api.Endpoints;
using MySpot.App;
using MySpot.Core;
using MySpot.Infrastructure;
using MySpot.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddCore();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.UseSerilog();

var app = builder.Build();

app.UseInfrastructure();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
// app.UseHttpsRedirection();
app.MapWeatherForecastApi();
app.MapParkingSpotsV1();
app.MapHomeV1();
app.MapUsersApiV1();

app.Run();
