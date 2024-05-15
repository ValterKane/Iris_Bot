using Iris_Bot.Bot.BotBrain;
using Iris_Bot.Models;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Iris_Bot.Bot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Iris_Bot.Bot.CommandProcessors
{
    internal class ChekInProc : ICommandProcessor
    {
        public bool CanProcess(CallbackQuery Command)
        {
            if (Command.Data.Contains("CheсkIn"))
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
                        var data = dataContext.ChekIns.Where(x => x.PhoneNumber == str).Include(o => o.IdActionNavigation).ToList();
                        if (data.Count > 0)
                        {
                            string result = "Мои записи:\n";
                            foreach (var item in data)
                            {
                                string status = item.Status == 0 ? "Запись активна" : "Запись неактивна";
                                result += $"Секция: {item.IdActionNavigation.Name}\n" +
                                    $"Дата записи: {item.Date}\n" +
                                    $"Статус записи: {status}\n\n";
                            }
                            await BotClient.SendTextMessageAsync(Command.From.Id, result);
                        }
                        else
                        {
                            await BotClient.SendTextMessageAsync(Command.From.Id, "Вы пока еще никуда не записывались через данную систему!");
                        }
                    }
                }
                if (_cmd == "Num")
                {
                    if (!Regex.IsMatch(BotMemory.ShortMemmory[Command.From.Id].CommandAtr, @"^\d+$"))
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
                                BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "CheсkIn:Pass", CommandAtr = string.Empty };
                            else
                                BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "CheсkIn:Pass", CommandAtr = string.Empty });
                        }
                    }
                   
                }
                if (_cmd == "Pass")
                {
                    if (BotMemory.LongMemmory[Command.From.Id].Password == BotMemory.ShortMemmory[Command.From.Id].CommandAtr)
                    {
                        ChekIn _temp = new ChekIn();
                        _temp.ChekInId = new Random().Next();
                        _temp.IdAction = BotMemory.TargetMemory[Command.From.Id];
                        _temp.PhoneNumber = BotMemory.LongMemmory[Command.From.Id].PhoneNumber;
                        _temp.Status = 0;
                        _temp.Date = DateTime.Now;

                        dataContext.ChekIns.Add(_temp);
                        try
                        {
                            dataContext.SaveChanges();
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Вы успешно записаны на секцию!");
                            BotMemory.ShortMemmory.Remove(Command.From.Id);
                        }
                        catch (Exception ex)
                        {
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Запись невозможна! Что-то пошло не так. Свяжитесь с администратором!");
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
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите номер телефона на который будет осуществляться запись");
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "CheсkIn:Num", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "CheсkIn:Num", CommandAtr = string.Empty });
                    }
                    else
                    {
                        ChekIn _temp = new ChekIn();
                        _temp.ChekInId = new Random().Next();
                        _temp.IdAction = _numOfAction;
                        _temp.PhoneNumber = BotMemory.LongMemmory[Command.From.Id].PhoneNumber;
                        _temp.Status = 0;
                        _temp.Date = DateTime.Now.Date;

                        dataContext.ChekIns.Add(_temp);
                        try
                        {
                            dataContext.SaveChanges();
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Вы успешно записаны на секцию!");
                            BotMemory.ShortMemmory.Remove(Command.From.Id);
                        }
                        catch (Exception ex)
                        {
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Запись невозможна! Что-то пошло не так. Свяжитесь с администратором!");
                        }
                    }
                    

                }
            }


        }
    }
}
