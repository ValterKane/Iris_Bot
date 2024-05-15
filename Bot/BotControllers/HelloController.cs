
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Iris_Bot.Bot.BotControllers
{
    internal class HelloController
    {

        public Message LastMessage { get; private set; }

        public InlineKeyboardMarkup InlineKeyboardMarkup { get; private set; }

        public HelloController()
        {

        }

        public string Processing()
        {
            InlineKeyboardMarkup = ProcessingMarkup();
            return "Добро пожаловать в Автономного помощника досугового центра 'ИРиС'\nЧем я могу помочь?";
        }

        private InlineKeyboardMarkup ProcessingMarkup()
        {
            InlineKeyboardButton Bttn_1 = new("da");
            InlineKeyboardButton Bttn_2 = new("da");
            InlineKeyboardButton Bttn_3 = new("da");
            InlineKeyboardButton Bttn_4 = new("da");

            Bttn_1.Text = "Просмотреть возможные факультативы";
            Bttn_2.Text = "ИРиС на картах";
            Bttn_3.Text = "Связаться с администратором";
            Bttn_4.Text = "Мой клиентский профиль";

            Bttn_1.CallbackData = "Faculty:All";
            Bttn_2.CallbackData = "Map";
            Bttn_3.CallbackData = "CallToAdmin";
            Bttn_4.CallbackData = "MyProfile:Hello";

            InlineKeyboardButton[] row_1 = new InlineKeyboardButton[] { Bttn_1 };
            InlineKeyboardButton[] row_2 = new InlineKeyboardButton[] { Bttn_2, Bttn_3 };
            InlineKeyboardButton[] row_3 = new InlineKeyboardButton[] { Bttn_4 };

            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[][] { row_1, row_2, row_3 };

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
