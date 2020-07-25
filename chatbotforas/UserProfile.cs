using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Mail;

namespace chatbotforas
{
    public class UserProfile
    {
        public string Error { get; set; }
        public string Solution { get; set; }
        public string Problem { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }

        public string LuisResult { get; set; }
    }
}
