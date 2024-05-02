namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Events;

public class Collision : WiredTriggerBase, IWired
{
    public Collision(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION) => this.Room.WiredHandler.TrgCollision += this.OnFurniCollision;

    private void OnFurniCollision(object obj, ItemTriggeredEventArgs args) => this.Room.WiredHandler.ExecutePile(this.Item.Coordinate, args.User, args.Item);

    public override void Dispose()
    {
        base.Dispose();

        this.Room.WiredHandler.TrgCollision -= this.OnFurniCollision;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
    }
}
