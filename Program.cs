using System.Net.Sockets;
using Lesson3_CNLTWeb.Data;
using Lesson3_CNLTWeb.Middleware;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("RoomManagement")
    ?? throw new InvalidOperationException("Connection string 'RoomManagement' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<RoomRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
        logger.LogError(ex, "Cannot connect to or initialize the database. Check that SQL LocalDB is running.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

try
{
    app.Run();
}
catch (IOException ex) when (ex.InnerException is AddressInUseException or SocketException)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Cannot start the web app: the localhost port is already in use.");
    Console.WriteLine("Stop the old instance or close the terminal that is running the app, then run dotnet run again.");
    Console.ResetColor();
    throw;
}
