namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

public class GiveScoreTeam : WiredActionBase, IWiredEffect, IWired
{
    private int _currentGameCount;

    public GiveScoreTeam(Item item, Room room) : base(item, room, (int)WiredActionType.GIVE_SCORE_TO_PREDEFINED_TEAM)
    {
        this._currentGameCount = 0;
        this.Room.GameManager.OnGameStart += this.OnGameStart;

        this.DefaultIntParams(new int[] { 1, 1, (int)TeamType.Red });
    }

    private void OnGameStart(object sender, EventArgs e) => this._currentGameCount = 0;

    public override bool OnCycle(RoomUser user, Item item)
    {
        var scoreToGive = this.GetIntParam(0);
        var maxCountPerGame = this.GetIntParam(1);
        var team = (TeamType)this.GetIntParam(2);

        if (maxCountPerGame <= this._currentGameCount)
        {
            return false;
        }

        this._currentGameCount++;
        this.Room.GameManager.AddPointToTeam(team, scoreToGive, user);

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();

        this.Room.GameManager.OnGameStart -= this.OnGameStart;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var scoreToGive = this.GetIntParam(0);
        var maxCountPerGame = this.GetIntParam(1);
        var team = this.GetIntParam(2);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, team.ToString(), maxCountPerGame.ToString() + ":" + scoreToGive.ToString(), false, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        var dataSplit = wiredTriggerData.Split(':');

        if (dataSplit.Length != 3)
        {
            return;
        }

        if (int.TryParse(dataSplit[1], out var score))
        {
            this.SetIntParam(0, score);
        }

        if (int.TryParse(dataSplit[0], out var maxCount))
        {
            this.SetIntParam(1, maxCount);
        }

        if (int.TryParse(wiredTriggerData2, out var team))
        {
            this.SetIntParam(2, team);
        }
    }
}
