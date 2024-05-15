using Iris_Bot.Bot.BotBrain;
using Iris_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Iris_Bot.Bot.CommandProcessors
{
    internal class CallToAdminProc : ICommandProcessor
    {
        public bool CanProcess(CallbackQuery Command)
        {
            if (Command.Data.Contains("CallToAdmin"))
            {
                return true;
            }
            else
                return false;
        }

        public async void ProcessCommand(ITelegramBotClient BotClient, CallbackQuery Command, Memory BotMemory)
        {
            IrisDbContext dataContext = new();
            var data = dataContext.Clients.Where(x => x.Status == "Администратор").FirstOrDefault();
            if (data != null)
            {
                await BotClient.SendContactAsync(Command.From.Id, data.PhoneNumber, data.FirstName);
            }
            else
            {
                await BotClient.SendTextMessageAsync(Command.From.Id, "Прошу прощения, но связаться с администратором не выйдет! Обратитесь в группу VK:(https://vk.com/center_iris.oskol)");
            }
            
        }
    }
}
