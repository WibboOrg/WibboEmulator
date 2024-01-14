namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HighScore : WiredActionBase, IWired, IWiredEffect
{
    public HighScore(Item item, Room room) : base(item, room, -1)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.Client == null)
        {
            return false;
        }

        var scores = this.ItemInstance.Scores;

        if (scores.TryGetValue(user.GetUsername(), out var score))
        {
            scores[user.GetUsername()] = score + 1;
        }
        else
        {
            scores.Add(user.GetUsername(), 1);
        }

        this.ItemInstance.UpdateState(false);

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        this.SaveToDatabase(dbClient);
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var triggerItems = "";

        foreach (var score in this.ItemInstance.Scores.OrderByDescending(x => x.Value))
        {
            triggerItems += score.Key + ":" + score.Value + ";";
        }

        triggerItems = triggerItems.TrimEnd(';');

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, triggerItems, false, null, this.Delay);
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

            if (!this.ItemInstance.Scores.ContainsKey(username))
            {
                this.ItemInstance.Scores.Add(username, score);
            }
        }
    }

    public override void OnTrigger(GameClient session)
    {
        _ = int.TryParse(this.ItemInstance.ExtraData, out var numMode);

        if (numMode != 1)
        {
            numMode = 1;
        }
        else
        {
            numMode = 0;
        }

        this.ItemInstance.ExtraData = numMode.ToString();
        this.ItemInstance.UpdateState(false);
    }
}
