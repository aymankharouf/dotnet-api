using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/api/customers", async (AppDbContext db) => {
    var customers = await db.Customers.ToListAsync();
    return Results.Ok(customers);
}).WithName("GetCustomers");
app.MapGet("/api/customer/{id}", async (AppDbContext db, Guid id) => {
    var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id);
    return Results.Ok(customer);
}).WithName("GetCustomerById");
app.MapPost("/api/customer", async (AppDbContext db, Customer customer) => {
    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetCustomerById", new {Id = customer.Id}, customer);
});
app.MapPut("/api/customer/{id}", async (AppDbContext db, Customer customer) => {
    db.Customers.Update(customer);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetCustomerById", new {Id = customer.Id}, customer);
});
app.MapDelete("/api/customer", async (AppDbContext db, Guid id) => {
    var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id);
    if (customer is null) return Results.NotFound();
    db.Customers.Remove(customer);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateTime.Now.AddDays(index),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

app.Run();

// record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

