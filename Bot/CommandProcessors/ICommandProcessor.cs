using Iris_Bot.Bot.BotBrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Iris_Bot.Bot.CommandProcessors
{
    internal interface ICommandProcessor
    {
        void ProcessCommand(ITelegramBotClient BotClient, CallbackQuery Command, Memory BotMemory);
        bool CanProcess(CallbackQuery Command);
    }
}
