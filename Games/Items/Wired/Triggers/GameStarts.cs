namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class GameStarts : WiredTriggerBase, IWired
{
    private readonly RoomEventDelegate _gameStartsDeletgate;

    public GameStarts(Item item, Room room) : base(item, room, (int)WiredTriggerType.GAME_STARTS)
    {
        this._gameStartsDeletgate = new RoomEventDelegate(this.GameManager_OnGameStart);
        this.RoomInstance.GetGameManager().OnGameStart += this._gameStartsDeletgate;
    }

    private void GameManager_OnGameStart(object sender, EventArgs e) => this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, null, null);

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetWiredHandler().GetRoom().GetGameManager().OnGameStart -= this._gameStartsDeletgate;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
    }

    public void LoadFromDatabase(DataRow row)
    {
    }
}
