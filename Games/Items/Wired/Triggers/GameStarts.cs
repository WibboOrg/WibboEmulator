namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class GameStarts : WiredTriggerBase, IWired
{
    public GameStarts(Item item, Room room) : base(item, room, (int)WiredTriggerType.GAME_STARTS) => this.Room.GameManager.OnGameStart += this.OnGameStart;

    private void OnGameStart(object sender, EventArgs e) => this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, null, null);

    public override void Dispose()
    {
        base.Dispose();

        this.Room.GameManager.OnGameStart -= this.OnGameStart;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
    }
}
