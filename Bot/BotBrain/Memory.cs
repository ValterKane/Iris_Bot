using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Iris_Bot.Bot.Commands;
using Iris_Bot.Models;

namespace Iris_Bot.Bot.BotBrain
{
    internal class Memory
    {
        private static readonly Lazy<Memory> lazy = new Lazy<Memory>(() => new Memory());

        public Dictionary<long, Client> LongMemmory { get; set; } = new Dictionary<long, Client>();

        public Dictionary<long, int> TargetMemory { get; set; } = new Dictionary<long, int>();

        public Dictionary<long, int> RatingMemory { get; set; } = new Dictionary<long, int>();

        public Dictionary<long , Command> ShortMemmory { get; set; } = new Dictionary<long , Command>();

        private Memory()
        {
            
        }

        public static Memory GetInstance()
        {
            return lazy.Value;
        }

    }
}
