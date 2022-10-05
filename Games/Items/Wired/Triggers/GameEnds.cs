namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class GameEnds : WiredTriggerBase, IWired
{
    private readonly RoomEventDelegate _gameEndsDeletgate;

    public GameEnds(Item item, Room room) : base(item, room, (int)WiredTriggerType.GAME_ENDS)
    {
        this._gameEndsDeletgate = new RoomEventDelegate(this.GameManager_OnGameEnd);
        this.RoomInstance.GetGameManager().OnGameEnd += this._gameEndsDeletgate;
    }

    private void GameManager_OnGameEnd(object sender, EventArgs e) => this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, null, null);

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetWiredHandler().GetRoom().GetGameManager().OnGameEnd -= this._gameEndsDeletgate;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
    }

    public void LoadFromDatabase(DataRow row)
    {
    }
}
