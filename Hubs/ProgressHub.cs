using Microsoft.AspNetCore.SignalR;

namespace PAMTestFlow  // Replace YourProjectName with your actual namespace
{
    public class ProgressHub : Hub
    {
        // This method will be used to send progress updates to clients
        public async Task SendProgressUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
