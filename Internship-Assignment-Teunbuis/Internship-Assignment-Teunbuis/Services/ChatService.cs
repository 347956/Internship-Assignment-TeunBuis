using DAL.Models;
using Microsoft.AspNetCore.SignalR;


namespace Internship_Assignment_Teunbuis.Services
{
    public class ChatService : Hub
    {
        public Task SendMessage(MessageModel messageModel)
        {
            string username = messageModel.UserName;
            string message = messageModel.Content;
            return Clients.All.SendAsync("RecieveMessage", username, message);
        }
    }
}
