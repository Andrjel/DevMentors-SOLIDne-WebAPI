using MySpot.Api.Endpoints;
using MySpot.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddScoped(sp => new ReservationsService());

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
