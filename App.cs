using Iris_Bot.Bot.BotClient;
using Iris_Bot.Bot.BotControllers;
using Iris_Bot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iris_Bot
{
    public class App
    {

        public static void Main(string[] args)
        {
            BotApp botApp = new BotApp(new Bot.BotSettings.Settings().BotAPI,Console.WriteLine);
            CancellationTokenSource cts = new CancellationTokenSource();
            botApp.BotStart(cts);

            Console.ReadLine();

            cts.Cancel();

        }
    }
}
