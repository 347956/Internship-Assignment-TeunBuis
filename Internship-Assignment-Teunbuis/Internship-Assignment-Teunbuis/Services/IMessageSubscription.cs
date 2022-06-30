namespace Internship_Assignment_Teunbuis.Services
{
    public interface IMessageSubscription
    {
        Task RecieveMessageAsync(string newmessagequeue);
    }
}