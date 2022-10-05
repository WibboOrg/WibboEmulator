namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

public class GiveScore : WiredActionBase, IWiredEffect, IWired
{
    private int currentGameCount;
    private RoomEventDelegate delegateFunction;

    public GiveScore(Item item, Room room) : base(item, room, (int)WiredActionType.GIVE_SCORE)
    {
        this.currentGameCount = 0;
        this.delegateFunction = new RoomEventDelegate(this.OnGameStart);
        this.RoomInstance.GetGameManager().OnGameStart += this.delegateFunction;

        this.IntParams.Add(1);
        this.IntParams.Add(1);
    }

    private void OnGameStart(object sender, EventArgs e) => this.currentGameCount = 0;

    public override bool OnCycle(RoomUser user, Item item)
    {
        var scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        if (user == null || user.Team == TeamType.NONE || maxCountPerGame <= this.currentGameCount)
        {
            return false;
        }

        this.currentGameCount++;
        this.RoomInstance.GetGameManager().AddPointToTeam(user.Team, scoreToGive, user);

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetGameManager().OnGameStart -= this.delegateFunction;
        this.delegateFunction = null;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, scoreToGive.ToString(), maxCountPerGame.ToString(), false, null, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out var maxCount))
        {
            this.IntParams.Add(maxCount);
        }

        if (int.TryParse(row["trigger_data_2"].ToString(), out var score))
        {
            this.IntParams.Add(score);
        }
    }
}
