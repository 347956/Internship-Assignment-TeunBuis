using DAL.Models;
using Internship_Assignment_Teunbuis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Validation;
using System.Net.WebSockets;
using System.Text;

namespace Internship_Assignment_Teunbuis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly DataContext dataContext;
        private readonly MessageQueue messageQueue;
        private readonly MessageSubscription subscription;
        private readonly ILogger<MessageController> logger;
        private ChatService chatService;

        public MessageController(DataContext dataContext, IConfiguration configuration, ILogger<MessageController> logger)
        {
            this.dataContext = dataContext;
            this.messageQueue = new MessageQueue(configuration);
            this.subscription = new MessageSubscription(configuration);
            this.logger = logger;
            this.chatService = new ChatService();
        }
        [HttpGet]
        public async Task<ActionResult<List<MessageModel>>> get()
        {
            List<MessageModel> messageModels = await dataContext.Messages.ToListAsync();
            return Ok(messageModels.OrderByDescending(messageModel => messageModel.Date));
        }
        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                logger.Log(LogLevel.Information, "WebSocket connection established");
                await Echo(webSocket);

            }
        }

        [HttpPost]
        public async Task<ActionResult> PostMessage(MessageModel messageModel)
        {
            messageModel = CheckMessage(messageModel);
            string messagequeue = "newmessagequeue";
            //sends a message to the service bus
            await messageQueue.SendMessageAsync(messageModel, messagequeue);
            //recieve messages from the servicebus
            await subscription.RecieveMessageAsync(messagequeue);
            try
            {
                dataContext.Messages.Add(messageModel);
                await dataContext.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine(e);
            }
            await chatService.SendMessage(messageModel);
            return Ok();
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            logger.Log(LogLevel.Information, "message recieved from Client");

            while (!result.CloseStatus.HasValue)
            {
                var serverMsg = Encoding.UTF8.GetBytes($"Server: Hello. You said: {Encoding.UTF8.GetString(buffer)}");
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                logger.Log(LogLevel.Information, "Message sent to Client");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                logger.Log(LogLevel.Information, "Message received from Client");

            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            logger.Log(LogLevel.Information, "WebSocket connection closed");
        }

        private MessageModel CheckMessage(MessageModel messageModel)
        {
            if(messageModel.UserName == null)
            {
                messageModel.UserName = "Anymous";
            }
            if(messageModel.Content == null)
            {
                messageModel.Content = "N/A";
            }
            if(messageModel.Date.Equals(null))
            {
                messageModel.Date = DateTime.Now;
            }
            return messageModel;
        }
    }
}
