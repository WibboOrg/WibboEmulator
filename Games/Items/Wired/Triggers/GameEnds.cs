namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class GameEnds : WiredTriggerBase, IWired
{
    public GameEnds(Item item, Room room) : base(item, room, (int)WiredTriggerType.GAME_ENDS) => this.Room.GameManager.OnGameEnd += this.OnGameEnd;

    private void OnGameEnd(object sender, EventArgs e) => this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, null, null);

    public override void Dispose()
    {
        base.Dispose();

        this.Room.GameManager.OnGameEnd -= this.OnGameEnd;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
    }
}
