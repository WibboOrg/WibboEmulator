using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms.AI.Responses;
using Butterfly.Game.Rooms.AI;

namespace Butterfly.Game.Bots
{
    public class BotManager
    {
        private readonly List<BotResponse> _responses;

        public BotManager()
        {
            _responses = new List<BotResponse>();
        }

        public void Init()
        {
            if (_responses.Count > 0)
                _responses.Clear();
        }

        public BotResponse GetResponse(BotAIType type, string message)
        {
            foreach (BotResponse response in _responses.Where(x => x.AiType == type).ToList())
            {
                if (response.KeywordMatched(message))
                {
                    return response;
                }
            }

            return null;
        }
    }
}
