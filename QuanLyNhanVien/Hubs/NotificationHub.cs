using Microsoft.AspNetCore.SignalR;

namespace QuanLyNhanVien.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task NotifyMobileAccess(string deviceInfo)
        {
            await Clients.All.SendAsync("ReceiveMobileAccess", deviceInfo);
        }
    }
}