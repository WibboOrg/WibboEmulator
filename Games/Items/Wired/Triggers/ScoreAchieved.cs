namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class ScoreAchieved : WiredTriggerBase, IWired
{
    public ScoreAchieved(Item item, Room room) : base(item, room, (int)WiredTriggerType.SCORE_ACHIEVED)
    {
        this.Room.GameManager.OnScoreChanged += this.OnScoreChanged;

        this.DefaultIntParams(0);
    }

    private void OnScoreChanged(object sender, TeamScoreChangedEventArgs e)
    {
        var scoreLevel = this.GetIntParam(0);
        if (e.Points <= scoreLevel - 1)
        {
            return;
        }

        this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, e.User, null);
    }

    public override void Dispose()
    {
        this.Room.GameManager.OnScoreChanged -= this.OnScoreChanged;

        base.Dispose();
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var scoreLevel = this.GetIntParam(0);
        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, scoreLevel.ToString());
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (int.TryParse(wiredTriggerData, out var score))
        {
            this.SetIntParam(0, score);
        }
    }
}
