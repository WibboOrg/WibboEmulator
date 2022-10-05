namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HighScore : WiredActionBase, IWired, IWiredEffect
{
    public HighScore(Item item, Room room) : base(item, room, -1)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.GetClient() == null)
        {
            return false;
        }

        var Scores = this.ItemInstance.Scores;

        var ListUsernameScore = new List<string>() { user.GetUsername() };

        if (Scores.ContainsKey(ListUsernameScore[0]))
        {
            Scores[ListUsernameScore[0]] += 1;
        }
        else
        {
            Scores.Add(ListUsernameScore[0], 1);
        }

        this.RoomInstance.SendPacket(new ObjectUpdateComposer(this.ItemInstance, this.RoomInstance.RoomData.OwnerId));

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        this.SaveToDatabase(dbClient);
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var triggerItems = "";

        foreach (var score in this.ItemInstance.Scores.OrderByDescending(x => x.Value))
        {
            triggerItems += score.Key + ":" + score.Value + ";";
        }

        triggerItems = triggerItems.TrimEnd(';');

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, triggerItems, false, null, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        var triggerData = row["trigger_data"].ToString();

        if (triggerData is null or "")
        {
            return;
        }

        foreach (var data in triggerData.Split(';'))
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
        int.TryParse(this.ItemInstance.ExtraData, out var NumMode);

        if (NumMode != 1)
        {
            NumMode = 1;
        }
        else
        {
            NumMode = 0;
        }

        this.ItemInstance.ExtraData = NumMode.ToString();
        this.ItemInstance.UpdateState(false, true);
    }
}
