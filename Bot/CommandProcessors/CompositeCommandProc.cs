using Iris_Bot.Bot.BotBrain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Iris_Bot.Bot.CommandProcessors
{
    internal class CompositeCommandProc : ICommandProcessor
    {
        private readonly List<ICommandProcessor> _processors = new();

        public bool CanProcess(CallbackQuery Command)
        {
            return _processors.Any(x => x.CanProcess(Command));
        }

        public void ProcessCommand(ITelegramBotClient BotClient, CallbackQuery Command, Memory BotMemory)
        {
            if (!CanProcess(Command)) return;

            List<ICommandProcessor> procs = _processors.Where(x => x.CanProcess(Command)).ToList();

            foreach (ICommandProcessor? processor in procs)
            {
                processor.ProcessCommand(BotClient, Command, BotMemory);
            }
        }

        public void Register(ICommandProcessor processor)
        {
            _processors.Add(processor);
        }

    }
}
