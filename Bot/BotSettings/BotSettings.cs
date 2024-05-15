using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iris_Bot.Bot.BotSettings
{
    public class Settings
    {
        private string bot_API = "7028879801:AAFHxjqtATfWx0k_FWlWMeWUE_X6WwFMdFg";

        public string BotAPI { get { return bot_API; } }

        public string BotName { get; } = "Автономный помощник 'ИРиС'";

        public string BotAddress { get; } = "@st_iris_bot";
    }
}
