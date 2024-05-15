using Iris_Bot.Bot.BotBrain;
using Iris_Bot.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Iris_Bot.Bot.Commands;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Iris_Bot.Bot.CommandProcessors
{
    internal class FeedbackProc : ICommandProcessor
    {
        public bool CanProcess(CallbackQuery Command)
        {
            if (Command.Data.Contains("FeedBack"))
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
                if (_cmd == "My")
                {
                    if (BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                    {
                        string str = BotMemory.LongMemmory[Command.From.Id].PhoneNumber;
                        List<Feedback> data = dataContext.Feedbacks.Where(x => x.PhoneNumber == str).Include(o => o.Action).ToList();
                        if (data.Count > 0)
                        {
                            string result = "Мои записи:\n";
                            foreach (Feedback? item in data)
                            {
                                result += $"Секция: {item.Action.Name}\n" +
                                    $"Дата записи: {item.DateOfRquest}\n" +
                                    $"Текст отзыва: {item.TextRequest}\n" +
                                    $"Оценка: {item.Rating}\n\n";
                                await BotClient.SendTextMessageAsync(Command.From.Id, result, replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                                {
                                new InlineKeyboardButton[]
                                {
                                    new("edit")
                                    {
                                        Text = "Редактировать",
                                        CallbackData = $"Feedback:Edit-{item.ActionId}",
                                    }
                                }
                            }));
                            }
                            
                        }
                        else
                        {
                            await BotClient.SendTextMessageAsync(Command.From.Id, "У Вас пока нет оставленных отзывов");
                        }
                    }
                }
                if (_cmd == "Num")
                {
                    if (!Regex.IsMatch(BotMemory.ShortMemmory[Command.From.Id].CommandAtr, @"7[0-9]{3}[0-9]{3}[0-9]{2}[0-9]{2}"))
                    {
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Упс. Похоже это не номер...", replyMarkup: new InlineKeyboardMarkup(
                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new("back")
                                        {
                                            Text = "Вернуться",
                                            CallbackData = "Hello",
                                        }
                                    }
                                }
                            ));
                    }
                    else
                    {
                        var data = dataContext.Clients.Where(x => x.PhoneNumber == BotMemory.ShortMemmory[Command.From.Id].CommandAtr).FirstOrDefault();
                        if (data != null)
                        {
                            BotMemory.LongMemmory.TryAdd(Command.From.Id, data);
                            await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите пароль");
                            if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                                BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "FeedBack:Pass", CommandAtr = string.Empty };
                            else
                                BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "FeedBack:Pass", CommandAtr = string.Empty });
                        }
                        else
                        {
                            await BotClient.SendTextMessageAsync(Command.From.Id, "Похоже мне не знаком этот номер. Давайте для начала зарегистрируемся");
                            if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                                BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Hello", CommandAtr = string.Empty };
                            else
                                BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Hello", CommandAtr = string.Empty });
                        }
                    }
                    
                }
                if (_cmd == "Pass")
                {
                    if (BotMemory.LongMemmory[Command.From.Id].Password == BotMemory.ShortMemmory[Command.From.Id].CommandAtr)
                    {
                        if (BotMemory.RatingMemory.ContainsKey(Command.From.Id))
                        {
                            BotMemory.RatingMemory.Remove(Command.From.Id);
                        }
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Поставьте оценку от 0 до 10");
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "FeedBack:Rating", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "FeedBack:Rating", CommandAtr = string.Empty });
                    }
                }
                if (_cmd == "Rating")
                {
                    if (Regex.IsMatch(BotMemory.ShortMemmory[Command.From.Id].CommandAtr, @"^\d+$"))
                    {
                        int rating = Convert.ToInt32(BotMemory.ShortMemmory[Command.From.Id].CommandAtr);
                        if (rating > 10 || rating < 0)
                        {
                            
                            await BotClient.SendTextMessageAsync(Command.From.Id, "Поставьте оценку от 0 до 10");
                            if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                                BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "FeedBack:Rating", CommandAtr = string.Empty };
                            else
                                BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "FeedBack:Rating", CommandAtr = string.Empty });
                        }
                        else
                        {
                            BotMemory.RatingMemory.TryAdd(Command.From.Id, rating);
                            await BotClient.SendTextMessageAsync(Command.From.Id, "Опишите свои впечатления");
                            if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                                BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "FeedBack:TextFlow", CommandAtr = string.Empty };
                            else
                                BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "FeedBack:TextFlow", CommandAtr = string.Empty });
                        }
                    }
                }
                if (_cmd == "TextFlow")
                {
                    await BotClient.SendTextMessageAsync(Command.From.Id, $"Ваш отзыв на :\t{dataContext.Actions.Where(x =>x.IdAction == BotMemory.TargetMemory[Command.From.Id]).FirstOrDefault().Name}\t\n" +
                        $"Комментарий:{BotMemory.ShortMemmory[Command.From.Id].CommandAtr}\n" +
                        $"Оценка: {BotMemory.RatingMemory[Command.From.Id]}", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][]
                            {
                                new InlineKeyboardButton[]
                                {
                                    new("1")
                                    {
                                        Text = "Да",
                                        CallbackData = "FeedBack:Yes"
                                    },
                                    new("2")
                                    {
                                        Text = "Нет",
                                        CallbackData = "FeedBack:No"
                                    },
                                }
                            }
                            ));
                }
                if (_cmd == "Yes")
                {
                    if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                    {
                        Feedback _temp = new Feedback()
                        {
                            ActionId = BotMemory.TargetMemory[Command.From.Id],
                            Rating = BotMemory.RatingMemory[Command.From.Id],
                            TextRequest = BotMemory.ShortMemmory[Command.From.Id].CommandAtr,
                            DateOfRquest = DateTime.Now,
                            PhoneNumber = BotMemory.LongMemmory[Command.From.Id].PhoneNumber,
                        };

                        dataContext.Feedbacks.Add(_temp);
                        try
                        {
                            dataContext.SaveChanges();
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Ваш отзыв успешно добавлен!");
                            BotMemory.ShortMemmory.Remove(Command.From.Id);
                            BotMemory.TargetMemory.Remove(Command.From.Id);
                            BotMemory.RatingMemory.Remove(Command.From.Id);
                        }
                        catch (Exception ex)
                        {
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Запись отзыва невозможна! Что-то пошло не так. Свяжитесь с администратором!");
                        }
                    }
                }
            }
            if (Command.Data.Contains('-'))
            {
                _cmd = Command.Data.Substring(Command.Data.IndexOf('-') + 1);
                int _numOfAction = 0;
                if (int.TryParse(_cmd, out _numOfAction))
                {
                    if (!BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                    {
                        BotMemory.TargetMemory.TryAdd(Command.From.Id, _numOfAction);
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите номер с которого будет оставлен отзыв");
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "FeedBack:Num", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "FeedBack:Num", CommandAtr = string.Empty });
                    }
                }
            }
        }
    }
}
