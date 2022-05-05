using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butterfly.Game.Rooms.AI.Speech
{
    public class RandomSpeech
    {
        public int BotID;
        public string Message;

        public RandomSpeech(string Message, int BotID)
        {
            this.BotID = BotID;
            this.Message = Message;
        }
    }
}