using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Triggers
{
    public class Collision : WiredTriggerBase, IWired
    {
        private readonly UserAndItemDelegate delegateFunction;

        public Collision(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION)
        {
            this.delegateFunction = new UserAndItemDelegate(this.FurniCollision);
            this.RoomInstance.GetWiredHandler().TrgCollision += this.delegateFunction;
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

            this.RoomInstance.GetWiredHandler().TrgCollision -= this.delegateFunction;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
        }

        public void LoadFromDatabase(DataRow row)
        {
        }
    }
}
