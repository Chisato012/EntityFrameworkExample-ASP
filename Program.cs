using System.Net.Sockets;
using Lesson3_CNLTWeb.Data;
using Lesson3_CNLTWeb.Middleware;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("BookManagement")
    ?? throw new InvalidOperationException("Connection string 'BookManagement' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<BookRepository>();

var app = builder.Build();

// Tạo database BookManagement và bảng Book nếu chưa tồn tại
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
        logger.LogError(ex, "Không thể kết nối hoặc khởi tạo database. Kiểm tra SQL LocalDB đang chạy.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
    Console.WriteLine("Không thể khởi động web: cổng localhost đang được sử dụng.");
    Console.WriteLine("Dừng instance cũ (Ctrl+C) hoặc đóng terminal đang chạy ứng dụng, rồi chạy lại dotnet run.");
    Console.ResetColor();
    throw;
}
