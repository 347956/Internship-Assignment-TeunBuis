using DAL.Models;
using Internship_Assignment_Teunbuis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Validation;

namespace Internship_Assignment_Teunbuis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly DataContext dataContext;
        private readonly MessageQueue messageQueue;
        private readonly MessageSubscription subscription;

        public MessageController(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.messageQueue = new MessageQueue(configuration);
            this.subscription = new MessageSubscription(configuration);
        }
        [HttpGet]
        public async Task<ActionResult<List<MessageModel>>> get()
        {
            List<MessageModel> messageModels = await dataContext.Messages.ToListAsync();
            return Ok(messageModels.OrderByDescending(messageModel => messageModel.Date));
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
            return Ok();
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
