namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Utilities.Events;

public class ScoreAchieved : WiredTriggerBase, IWired
{
    private readonly TeamScoreChangedDelegate _scoreChangedDelegate;

    public ScoreAchieved(Item item, Room room) : base(item, room, (int)WiredTriggerType.SCORE_ACHIEVED)
    {
        this._scoreChangedDelegate = new TeamScoreChangedDelegate(this.OnScoreChanged);
        this.RoomInstance.GetGameManager().OnScoreChanged += this._scoreChangedDelegate;

        this.IntParams.Add(0);
    }

    private void OnScoreChanged(object sender, TeamScoreChangedEventArgs e)
    {
        var scoreLevel = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        if (e.Points <= scoreLevel - 1)
        {
            return;
        }

        this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, e.User, null);
    }

    public override void Dispose()
    {
        this.RoomInstance.GetWiredHandler().GetRoom().GetGameManager().OnScoreChanged -= this._scoreChangedDelegate;

        base.Dispose();
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var scoreLevel = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, scoreLevel.ToString(), false, null);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["trigger_data"].ToString(), out var score))
        {
            this.IntParams.Add(score);
        }
    }
}
