namespace Lesson3_CNLTWeb.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();

            // Log only room-management requests, skipping bootstrap, jquery, css, and js assets.
            if (path.Contains("/Room"))
            {
                Console.WriteLine($"[Time before action: {time}] Method: {method} - Path: {path}");
            }
            
            if (path == "/Room/Detail/0" || path == "/Room/Detail/-1")
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Room id is invalid");
                return;
            }

            await _next(context);

            if (path.Contains("/Room"))
            {
                var timeAfter = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Console.WriteLine($"Time After action: [{timeAfter}] - Method: {method} - Path: {path}");
            }
        }
    }
}
