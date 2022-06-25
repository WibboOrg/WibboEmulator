using System.Data;
using WibboEmulator.Game.Rooms.AI.Responses;
using WibboEmulator.Game.Rooms.AI;

namespace WibboEmulator.Game.Bots
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
