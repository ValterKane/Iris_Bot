using Iris_Bot.Bot.BotBrain;
using Iris_Bot.Bot.Commands;
using Iris_Bot.Models;
using K4os.Compression.LZ4.Internal;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Iris_Bot.Bot.CommandProcessors
{
    internal class MyProfileProc : ICommandProcessor
    {
        public bool CanProcess(CallbackQuery Command)
        {
            if (Command.Data.Contains("MyProfile"))
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
                
                if (_cmd == "Break")
                {
                    BotMemory.LongMemmory.Remove(Command.From.Id);
                    BotMemory.ShortMemmory.Remove(Command.From.Id);
                    await BotClient.SendTextMessageAsync(Command.From.Id, "Хорошо. Давай попробуем снова! Напишите 'Привет!'");
                   
                }
                
                if (_cmd == "Hello")
                {
                    if (!BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                    {
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите свой номер телефона:");
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Phone", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Phone", CommandAtr = string.Empty });
                    }
                    else
                    {
                        await BotClient.SendTextMessageAsync(Command.From.Id, $"Аккаунт найден {BotMemory.LongMemmory[Command.From.Id].PhoneNumber}. Укажите пароль", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][]
                            {
                                new InlineKeyboardButton[]
                                {
                                    new("1"){
                                       Text = "Авторизоваться заново",
                                       CallbackData = "MyProfile:Remove"

                                    }
                                }
                            }
                            ));
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Phone_Password", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Phone_Password", CommandAtr = string.Empty });
                    }

                }
                if (_cmd == "Remove")
                {
                    if (BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                    {
                        BotMemory.LongMemmory.Remove(Command.From.Id);
                        await BotClient.EditMessageTextAsync(Command.From.Id, Command.Message.MessageId, "Тогда давайте начнем всё с начала! Скажите 'Привет!'");
                        await BotClient.EditMessageReplyMarkupAsync(Command.From.Id, Command.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(
                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new("hello")
                                        {
                                            Text = "Привет!",
                                            CallbackData = "Hello"
                                        }
                                    }
                                }
                                ));
                    }
                }
                if (_cmd.Contains("Phone"))
                {
                    Client data = null;

                   
                    if (BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                    {
                        data = BotMemory.LongMemmory[Command.From.Id];
                    }
                    else
                    {
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                        {
                            Regex regex = new Regex("7[0-9]{3}[0-9]{3}[0-9]{2}[0-9]{2}");
                            string CommandAttr = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;

                            if (regex.IsMatch(CommandAttr))
                            {
                                data = dataContext.Clients.Where(x => x.PhoneNumber == BotMemory.ShortMemmory[Command.From.Id].CommandAtr).FirstOrDefault();
                                BotMemory.LongMemmory.Add(Command.From.Id, data);
                            }
                            else
                            {
                                await BotClient.SendTextMessageAsync(Command.From.Id, "Это не номер телефона! Убедитесь, что указываете номер корректно! (Например, 79991112233)", replyMarkup: new InlineKeyboardMarkup(
                                        new InlineKeyboardButton[][]
                                        {
                                    new InlineKeyboardButton[]
                                    {
                                        new("hello")
                                        {
                                            Text = "Прервать!",
                                            CallbackData = "Hello"
                                        }
                                    }}));
                                return;
                            }
                        }
                    }
                    if (data != null)
                    {
                        if (_cmd == "Phone_SaveTelegram")
                        {
                            data.TelegramId = Command.From.Id.ToString();
                            dataContext.Clients.Update(data);
                            dataContext.SaveChanges();
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Данный чат сохранен как основной для пользователя: {data.PhoneNumber}");
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Авторизация пройдена. Ваши данные:\nФ.И.О:{data.FirstName} {data.SecondName} {data.LastName}\n" +
                                        $"Статус клиента: {data.Status}\nАктуальный ID для обратной связи: {data.TelegramId}", replyMarkup: new InlineKeyboardMarkup(
                                            new InlineKeyboardButton[][]
                                            {
                                                new InlineKeyboardButton[]
                                                {
                                                    new("1")
                                                    {
                                                        Text = "Запомнить данный чат, как основной",
                                                        CallbackData = "MyProfile:Phone_SaveTelegram"
                                                    },
                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    new("2")
                                                    {
                                                        Text = "Показать мои отзывы",
                                                        CallbackData = "FeedBack:My"
                                                    },
                                                    new("3")
                                                    {
                                                        Text = "Показать мои записи",
                                                        CallbackData = "CheсkIn:My"
                                                    }

                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    new("4")
                                                    {
                                                        Text = "Вернуться на главную",
                                                        CallbackData = "Hello"
                                                    }
                                                }
                                            }));
                            BotMemory.ShortMemmory.Remove(Command.From.Id);

                        }
                        else
                        {
                            if (_cmd != "Phone_Password")
                            {
                                await BotClient.SendTextMessageAsync(Command.From.Id, "Пользователь найден. Укажите пароль");
                                BotMemory.ShortMemmory[Command.From.Id].CommandName = "MyProfile:Phone_Password";
                                BotMemory.ShortMemmory[Command.From.Id].CommandAtr = string.Empty;
                            }
                            else
                            {

                                string pwd = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;
                                if (pwd == data.Password)
                                {
                                    await BotClient.SendTextMessageAsync(Command.From.Id, $"Авторизация пройдена. Ваши данные:\nФ.И.О:{data.FirstName} {data.SecondName} {data.LastName}\n" +
                                        $"Статус клиента: {data.Status}\nАктуальный ID для обратной связи: {data.TelegramId}", replyMarkup: new InlineKeyboardMarkup(
                                            new InlineKeyboardButton[][]
                                            {
                                                new InlineKeyboardButton[]
                                                {
                                                    new("1")
                                                    {
                                                        Text = "Запомнить данный чат, как основной",
                                                        CallbackData = "MyProfile:Phone_SaveTelegram"
                                                    },
                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    new("2")
                                                    {
                                                        Text = "Показать мои отзывы",
                                                        CallbackData = "FeedBack:My"
                                                    },
                                                    new("3")
                                                    {
                                                        Text = "Показать мои записи",
                                                        CallbackData = "CheсkIn:My"
                                                    }

                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    new("4")
                                                    {
                                                        Text = "Вернуться на главную",
                                                        CallbackData = "Hello"
                                                    }
                                                }
                                            }));
                                    BotMemory.ShortMemmory.Remove(Command.From.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Такого пользователя не найдено в системе. Хотите зарегистрироваться?", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][]
                            {
                                new InlineKeyboardButton[]
                                {
                                    new("1")
                                    {
                                        Text = "Да",
                                        CallbackData = "MyProfile:Yes_Num",
                                    },
                                    new("1")
                                    {
                                        Text = "Нет",
                                        CallbackData = "MyProfile:No",
                                    },

                                }
                            }));
                    }


                }

                if (_cmd.Contains("Yes"))
                {
                    if (_cmd == "Yes_Num")
                    {
                        if (BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                        {
                            if (BotMemory.LongMemmory[Command.From.Id] == null)
                            {
                                BotMemory.LongMemmory[Command.From.Id] = new Client();
                            }
                        }
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                        {
                            if (BotMemory.ShortMemmory[Command.From.Id].CommandName == "MyProfile:Phone")
                            {
                                BotMemory.LongMemmory[Command.From.Id].PhoneNumber = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;
                                await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите имя: ");

                                if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                                    BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Yes_FName", CommandAtr = string.Empty };
                                else
                                    BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Yes_FName", CommandAtr = string.Empty });
                            }
                        }
                        else
                        {
                            await BotClient.EditMessageTextAsync(Command.From.Id, Command.Message.MessageId, "Тогда давайте начнем всё с начала! Скажите 'Привет!'");
                            await BotClient.EditMessageReplyMarkupAsync(Command.From.Id, Command.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(
                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new("hello")
                                        {
                                            Text = "Привет!",
                                            CallbackData = "Hello"
                                        }
                                    }
                                }
                                ));
                        }
                    }
                    if (_cmd == "Yes_FName")
                    {
                        BotMemory.LongMemmory[Command.From.Id].FirstName = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите отчество: ", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][] {
                                new InlineKeyboardButton[]
                                {
                                    new("back")
                                    {
                                        Text = "Прервать",
                                        CallbackData = "MyProfile:Break"
                                    }
                                }
                            }
                            ));
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Yes_SName", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Yes_SName", CommandAtr = string.Empty });
                    }
                    if (_cmd == "Yes_SName")
                    {
                        BotMemory.LongMemmory[Command.From.Id].SecondName = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите фамилию: ", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][] {
                                new InlineKeyboardButton[]
                                {
                                    new("back")
                                    {
                                        Text = "Прервать",
                                        CallbackData = "MyProfile:Break"
                                    }
                                }
                            }
                            ));
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Yes_LName", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Yes_LName", CommandAtr = string.Empty });
                    }
                    if (_cmd == "Yes_LName")
                    {
                        BotMemory.LongMemmory[Command.From.Id].LastName = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите дату рождения: ", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][] {
                                new InlineKeyboardButton[]
                                {
                                    new("back")
                                    {
                                        Text = "Прервать",
                                        CallbackData = "MyProfile:Break"
                                    }
                                }
                            }
                            ));
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Yes_DoB", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Yes_DoB", CommandAtr = string.Empty });
                    }
                    if (_cmd == "Yes_DoB")
                    {
                        BotMemory.LongMemmory[Command.From.Id].DoB = Convert.ToDateTime(BotMemory.ShortMemmory[Command.From.Id].CommandAtr);
                        await BotClient.SendTextMessageAsync(Command.From.Id, "Укажите пароль: ", replyMarkup: new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][] {
                                new InlineKeyboardButton[]
                                {
                                    new("back")
                                    {
                                        Text = "Прервать",
                                        CallbackData = "MyProfile:Break"
                                    }
                                }
                            }
                            ));
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                            BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Yes_Pwd", CommandAtr = string.Empty };
                        else
                            BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Yes_Pwd", CommandAtr = string.Empty });
                    }
                    if (_cmd == "Yes_Pwd")
                    {
                        BotMemory.LongMemmory[Command.From.Id].Password = BotMemory.ShortMemmory[Command.From.Id].CommandAtr;
                        await BotClient.SendTextMessageAsync(Command.From.Id, $"Проверьте введенные данные:\nФИО: {BotMemory.LongMemmory[Command.From.Id].FirstName} {BotMemory.LongMemmory[Command.From.Id].SecondName} {BotMemory.LongMemmory[Command.From.Id].LastName}\n" +
                            $"Дата рождения: {BotMemory.LongMemmory[Command.From.Id].DoB.Value.ToShortDateString()}\nНомер телефона: {BotMemory.LongMemmory[Command.From.Id].PhoneNumber}\n" +
                            $"Пароль: {BotMemory.LongMemmory[Command.From.Id].Password}", replyMarkup: new InlineKeyboardMarkup(
                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new("1")
                                        {
                                            Text = "Да",
                                            CallbackData = "MyProfile:Yes_Yes",

                                        },
                                        new("1")
                                        {
                                            Text = "Нет",
                                            CallbackData = "MyProfile:Yes_No",

                                        }
                                    }
                                }
                                ));

                    }
                    if (_cmd == "Yes_Yes")
                    {
                        if (BotMemory.LongMemmory.ContainsKey(Command.From.Id))
                        {
                            dataContext.Clients.Add(BotMemory.LongMemmory[Command.From.Id]);
                            dataContext.SaveChanges();
                            await BotClient.SendTextMessageAsync(Command.From.Id, $"Пользователь успешно сохранен. Привет, {BotMemory.LongMemmory[Command.From.Id].FirstName}\nНезабудь удалить сообщения с паролем! Я не храню его в таком виде!");
                            BotMemory.LongMemmory.Remove(Command.From.Id);


                        }

                    }
                    if (_cmd == "Yes_No")
                    {
                        if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                        {
                            if (BotMemory.ShortMemmory[Command.From.Id].CommandName == "MyProfile:Yes_Pwd") { }
                            await BotClient.SendTextMessageAsync(Command.From.Id, "Упс. Давай попробуем с самого начала\nУкажите свой номер телефона:");
                            if (BotMemory.ShortMemmory.ContainsKey(Command.From.Id))
                                BotMemory.ShortMemmory[Command.From.Id] = new Command() { CommandName = "MyProfile:Yes_Num", CommandAtr = string.Empty };
                            else
                                BotMemory.ShortMemmory.Add(Command.From.Id, new Command() { CommandName = "MyProfile:Yes_Num", CommandAtr = string.Empty });
                        }

                    }
                }

                if (_cmd.Contains("No"))
                {
                    await BotClient.EditMessageTextAsync(Command.From.Id, Command.Message.MessageId, "Тогда давайте начнем всё с начала! Скажите 'Привет!'", replyMarkup: new InlineKeyboardMarkup(
                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new("hello")
                                        {
                                            Text = "Привет!",
                                            CallbackData = "Hello"
                                        }
                                    }
                                }
                                ));
                }
            }

        }
    }
}
