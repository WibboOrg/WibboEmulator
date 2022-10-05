namespace WibboEmulator.Games.Rooms.AI.Responses;
using WibboEmulator.Games.Catalog.Utilities;

public class BotResponse
{
    public BotAIType AiType { get; set; }
    public List<string> Keywords { get; set; }
    public string ResponseText { get; set; }
    public string ResponseType { get; set; }
    public List<int> BeverageIds { get; private set; }

    public BotResponse(string botAi, string keywords, string responseText, string responseMode, string responseBeverages)
    {
        this.AiType = BotUtility.GetAIFromString(botAi);

        this.Keywords = new List<string>();
        foreach (var keyword in keywords.Split(','))
        {
            this.Keywords.Add(keyword.ToLower());
        }

        this.ResponseText = responseText;
        this.ResponseType = responseMode;

        this.BeverageIds = new List<int>();
        if (responseBeverages.Contains(','))
        {
            foreach (var vendingId in responseBeverages.Split(','))
            {
                try
                {
                    this.BeverageIds.Add(int.Parse(vendingId));
                }
                catch
                {
                    continue;
                }
            }
        }
        else if (!string.IsNullOrEmpty(responseBeverages) && int.Parse(responseBeverages) > 0)
        {
            this.BeverageIds.Add(int.Parse(responseBeverages));
        }
    }
}
