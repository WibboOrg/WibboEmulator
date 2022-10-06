namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class Collision : WiredTriggerBase, IWired
{
    private readonly UserAndItemDelegate _delegateFunction;

    public Collision(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION)
    {
        this._delegateFunction = new UserAndItemDelegate(this.FurniCollision);
        this.RoomInstance.GetWiredHandler().TrgCollision += this._delegateFunction;
    }

    private void FurniCollision(RoomUser user, Item item)
    {
        if (user == null)
        {
            return;
        }

        this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, item);
    }

    public override void Dispose()
    {
        base.Dispose();

        this.RoomInstance.GetWiredHandler().TrgCollision -= this._delegateFunction;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
    }

    public void LoadFromDatabase(DataRow row)
    {
    }
}
