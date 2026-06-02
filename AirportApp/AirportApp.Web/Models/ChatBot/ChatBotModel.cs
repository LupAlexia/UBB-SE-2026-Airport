using AirportApp.ClassLibrary.Entity.Dto;

namespace AirportApp.Web.Models.ChatBot
{
    public class ChatBotModel
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public List<MessageDTO> Messages { get; set; } = new ();
        public List<FAQOptionDTO> CurrentOptions { get; set; } = new ();
    }
}

