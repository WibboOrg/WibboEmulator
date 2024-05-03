namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HighScorePoints(Item item, Room room) : WiredActionBase(item, room, -1), IWired, IWiredEffect
{
    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.Client == null)
        {
            return false;
        }

        var scores = this.Item.Scores;

        if (scores.TryGetValue(user.Username, out var score))
        {
            if (user.WiredPoints > score)
            {
                scores[user.Username] = user.WiredPoints;
            }
        }
        else
        {
            scores.Add(user.Username, user.WiredPoints);
        }

        this.Item.UpdateState(false);

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();

        using var dbClient = DatabaseManager.Connection;
        this.SaveToDatabase(dbClient);
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var triggerItems = "";

        foreach (var score in this.Item.Scores.OrderByDescending(x => x.Value).Take(1000))
        {
            triggerItems += score.Key + ":" + score.Value + ";";
        }

        triggerItems = triggerItems.TrimEnd(';');

        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, triggerItems, false, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (wiredTriggerData is "")
        {
            return;
        }

        foreach (var data in wiredTriggerData.Split(';'))
        {
            var userData = data.Split(':');

            _ = int.TryParse(userData[^1], out var score);

            var username = "";
            for (var i = 0; i < userData.Length - 1; i++)
            {
                if (i == 0)
                {
                    username = userData[i];
                }
                else
                {
                    username += ':' + userData[i];
                }
            }

            _ = this.Item.Scores.TryAdd(username, score);
        }
    }

    public override void OnTrigger(GameClient session)
    {
        _ = int.TryParse(this.Item.ExtraData, out var numMode);

        if (numMode != 1)
        {
            numMode = 1;
        }
        else
        {
            numMode = 0;
        }

        this.Item.ExtraData = numMode.ToString();
        this.Item.UpdateState(false);
    }
}
