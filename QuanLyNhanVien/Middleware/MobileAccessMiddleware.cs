using Microsoft.AspNetCore.SignalR;
using QuanLyNhanVien.Hubs;

namespace QuanLyNhanVien.Middleware
{
    public class MobileAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MobileAccessMiddleware(RequestDelegate next, IHubContext<NotificationHub> hubContext)
        {
            _next = next;
            _hubContext = hubContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            // Check if request is from mobile device
            if (IsMobileDevice(userAgent) && !context.Request.Path.StartsWithSegments("/notificationHub"))
            {
                var deviceInfo = $"Mobile access detected: {userAgent} at {DateTime.Now:HH:mm:ss}";
                await _hubContext.Clients.All.SendAsync("ReceiveMobileAccess", deviceInfo);
            }

            await _next(context);
        }

        private bool IsMobileDevice(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return false;

            userAgent = userAgent.ToLower();

            // Common mobile device keywords
            string[] mobileKeywords = {
                "mobile", "android", "iphone", "ipad", "ipod", "blackberry",
                "windows phone", "opera mini", "opera mobi", "iemobile"
            };

            return mobileKeywords.Any(keyword => userAgent.Contains(keyword));
        }
    }
}