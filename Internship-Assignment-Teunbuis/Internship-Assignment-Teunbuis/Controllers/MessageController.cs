using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Validation;

namespace Internship_Assignment_Teunbuis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly DataContext dataContext;
        public MessageController(DataContext dataContext)
        {
            this.dataContext = dataContext;
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
            try
            {                
                dataContext.Messages.Add(messageModel);
                await dataContext.SaveChangesAsync();
            }
            catch(DbEntityValidationException e)
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
            if(messageModel.Date == null)
            {
                messageModel.Date = DateTime.Now;
            }
            return messageModel;
        }
    }
}
