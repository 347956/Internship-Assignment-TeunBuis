using DAL.Models;
using Microsoft.AspNetCore.SignalR;


namespace Internship_Assignment_Teunbuis.Services
{
    public class ChatService : Hub
    {
        public Task SendMessage(MessageModel messageModel)
        {
            return Clients.All.SendAsync("RecieveMessage", messageModel.UserName, messageModel.Content);
        }
    }
}
