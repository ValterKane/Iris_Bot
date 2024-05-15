using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Iris_Bot.Models;
using Telegram.Bot.Types.ReplyMarkups;
using Iris_Bot.Bot.BotBrain;

namespace Iris_Bot.Bot.CommandProcessors
{
    
    internal class FacultyProc : ICommandProcessor
    {
        
        public bool CanProcess(CallbackQuery Command)
        {
            if (Command.Data.Contains("Faculty"))
            {
                return true;
            }
            else
                return false;
        }

        public async void ProcessCommand(ITelegramBotClient BotClient, CallbackQuery Command, Memory BotMemory)
        {
            string _cmd = string.Empty;
            IrisDbContext dataContext = new();
            if (Command.Data.Contains(':'))
            {
                _cmd = Command.Data.Substring(Command.Data.IndexOf(':') + 1);
                if (_cmd == "All")
                {

                    List<ActionType> data = dataContext.ActionTypes.ToList();
                    await BotClient.SendTextMessageAsync(Command.From.Id, "Какие студии вам интересны?", replyMarkup: ReplyMarkup(data));
                }
                else
                {
                    int index = Convert.ToInt32(_cmd);
                    List<Models.Action> data = dataContext.Actions.Where(x => x.Type == index).ToList();
                    if (data.Count != 0)
                    {
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Для более подробого описания выберите факультатив:", replyMarkup: ReplyMarkup(data));
                    }
                    else
                    {
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Данных пока нет или они на обработке... Обратитесь позже!");
                    }

                }
            }
            if (Command.Data.Contains('-'))
            {
                _cmd = Command.Data.Substring(Command.Data.IndexOf('-') + 1);
                int index = Convert.ToInt32(_cmd);

                var data = dataContext.Actions.Where(x => x.IdAction == index).Include(u => u.Schedules).ThenInclude(o => o.IdDayNavigation).Include(u => u.ChekIns).Include(i => i.ActionPlans).
                    ThenInclude(i => i.IdPlanNavigation).ToList();
                string schedules = string.Empty;
                foreach (var sch in data[0].Schedules)
                {
                    schedules += $"{sch.IdDayNavigation.ShortDay}:{sch.Time}\n";
                }
                int TotalCapacity = (int)(data[0].Capacity - data[0].ChekIns.Count(x => x.Status != 1));
                string plansInfo = "Доступные тарифные планы:\n";
                foreach (var pl in data[0].ActionPlans)
                {
                    plansInfo += $"{pl.IdPlanNavigation.PlanName}: {pl.Cost} руб.\n";
                    
                }
                string descr = $"{data[0].Description}\n{schedules}\nРуководитель: {data[0].Chair}\nСредний рейтинг курса: {data[0].AvarageRating}\nВозрастное ограничение: {data[0].AgeAllow}\n" +
                    $"Количество свободных мест: {TotalCapacity}/{data[0].Capacity}\n\n{plansInfo}";
                await BotClient.SendTextMessageAsync(Command.From.Id, descr, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                {
                    new[] {new InlineKeyboardButton("1")
                    {
                        Text = "Записаться на курс?",
                        CallbackData = $"CheсkIn-{data[0].IdAction}",
                    }},
                    new[] {new InlineKeyboardButton("2")
                    {
                        Text = "Оставить отзыв о курсе?",
                        CallbackData = $"FeedBack-{data[0].IdAction}",
                    }
                    
                    }
                }));

            }
        }

        private InlineKeyboardMarkup ReplyMarkup(List<Models.Action> data)
        {
            InlineKeyboardMarkup replMrp;

            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[data.Count][];

            for (int i = 0; i < data.Count; i++)
            {
                keyboard[i] = new InlineKeyboardButton[]
                {
                        new($"{i}"){
                            Text = $"{data[i].IdAction}:{data[i].Name}",
                            CallbackData = $"Faculty-{data[i].IdAction}"
                        },
                    };
            }

            replMrp = new InlineKeyboardMarkup(keyboard);
            return replMrp;
        }

        private InlineKeyboardMarkup ReplyMarkup(List<ActionType> data)
        {
            InlineKeyboardMarkup replMrp;

            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[data.Count][];

            for (int i = 0; i < data.Count; i++)
            {
                keyboard[i] = new InlineKeyboardButton[]
                {
                        new($"{i}"){
                            Text = $"{data[i].IdType}:{data[i].Description}",
                            CallbackData = $"Faculty:{data[i].IdType}"
                        },
                    };
            }

            replMrp = new InlineKeyboardMarkup(keyboard);
            return replMrp;
        }
    }
      
}

