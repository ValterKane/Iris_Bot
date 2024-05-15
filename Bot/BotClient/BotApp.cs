using Iris_Bot.Bot.BotBrain;
using Iris_Bot.Bot.BotControllers;
using Iris_Bot.Bot.CommandProcessors;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Iris_Bot.Bot.BotClient
{
    public class BotApp
    {
        private readonly TelegramBotClient botClient;
        private CancellationTokenSource cts;
        private readonly Action<string> PrintLog;
        private readonly ReceiverOptions ReceiverOptions;
        private readonly Memory BotMemory = Memory.GetInstance();
        private readonly CompositeCommandProc CommandProccessor = new();

        public BotApp(string botAPI, Action<string> OutputForLog)
        {
            botClient = new(botAPI);
            PrintLog = OutputForLog;
            ReceiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>(),
            };
            CommandProccessor.Register(new FacultyProc());
            CommandProccessor.Register(new MyProfileProc());
            CommandProccessor.Register(new ChekInProc());
            CommandProccessor.Register(new FeedbackProc());
            CommandProccessor.Register(new CallToAdminProc());
        }

        public bool BotStart(CancellationTokenSource cts)
        {
            this.cts = cts;

            try
            {
                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: ReceiverOptions,
                    cancellationToken: this.cts.Token);

                PrintLog.Invoke($"Бот запущен...");

                return true;
            }
            catch (Exception ex)
            {
                PrintLog?.Invoke(ex.Message);
                return false;
            }
        }

        private async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            string? ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            PrintLog?.Invoke(ErrorMessage);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            long chatID;
            if (update.Message is Message message)
            {
                chatID = update.Message.Chat.Id;

                if (BotMemory.ShortMemmory.ContainsKey(chatID))
                {
                    
                    Commands.Command cmd = BotMemory.ShortMemmory[chatID];
                    cmd.CommandAtr = $"{message.Text}";
                    CallbackQuery callbackQuery = new();
                    callbackQuery.From = update.Message.From;
                    callbackQuery.Data = cmd.CommandName;
                    if (BotMemory.ShortMemmory[chatID].CommandName.Contains("Pass"))
                    {
                        client.DeleteMessageAsync(chatID, update.Message.MessageId);
                    }
                    CommandProccessor.ProcessCommand(botClient, callbackQuery, BotMemory);
                    PrintLog.Invoke($"Получена команда {callbackQuery.Data}");
                }
                else
                {
                    HelloController Controller = new();
                    await client.SendTextMessageAsync(chatID, Controller.Processing(), replyMarkup: Controller.InlineKeyboardMarkup);
                }
            }
            if (update.CallbackQuery != null)
            {
                chatID = update.CallbackQuery.From.Id;
                if (update.CallbackQuery.Data == "Hello")
                {
                    if (BotMemory.ShortMemmory.ContainsKey((long)chatID))
                    {
                        BotMemory.ShortMemmory.Remove((long)chatID);
                    }
                    HelloController Controller = new();
                    await client.SendTextMessageAsync(chatID, Controller.Processing(), replyMarkup: Controller.InlineKeyboardMarkup);
                }
                else
                {

                    PrintLog.Invoke($"Получена команда {update.CallbackQuery.Data}");
                    CommandProccessor.ProcessCommand(botClient, update.CallbackQuery, BotMemory);
                }
            }
        }
    }
}
