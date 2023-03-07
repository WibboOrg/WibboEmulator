namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
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
        this.RoomInstance.GameManager.OnGameStart += this.OnGameStart;

        this.IntParams.Add(1);
        this.IntParams.Add(1);
        this.IntParams.Add((int)TeamType.Red);
    }

    private void OnGameStart(object sender, EventArgs e) => this._currentGameCount = 0;

    public override bool OnCycle(RoomUser user, Item item)
    {
        var scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;
        var team = (TeamType)((this.IntParams.Count > 2) ? this.IntParams[2] : 1);

        if (maxCountPerGame <= this._currentGameCount)
        {
            return false;
        }

        this._currentGameCount++;
        this.RoomInstance.GameManager.AddPointToTeam(team, scoreToGive, user);

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.
        GameManager.OnGameStart -= this.OnGameStart;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var scoreToGive = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var maxCountPerGame = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;
        var team = (this.IntParams.Count > 2) ? this.IntParams[2] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, team.ToString(), maxCountPerGame.ToString() + ":" + scoreToGive.ToString(), false, null, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.IntParams.Clear();

        this.Delay = wiredDelay;

        var triggerData = wiredTriggerData;
        var triggerData2 = wiredTriggerData2;

        if (triggerData == null || !triggerData.Contains(':'))
        {
            return;
        }

        var dataSplit = triggerData.Split(':');

        if (int.TryParse(dataSplit[1], out var score))
        {
            this.IntParams.Add(score);
        }

        if (int.TryParse(dataSplit[0], out var maxCount))
        {
            this.IntParams.Add(maxCount);
        }

        if (int.TryParse(triggerData2, out var team))
        {
            this.IntParams.Add(team);
        }
    }
}
