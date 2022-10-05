using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired.Actions
{
    public class TeleportToItem : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
    {
        public TeleportToItem(Item item, Room room) : base(item, room, (int)WiredActionType.TELEPORT)
        {
            this.Delay = 1;
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (user == null)
            {
                return false;
            }

            if (this.Items.Count > 1)
            {
                Item roomItem = this.Items[WibboEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
                if (roomItem == null)
                {
                    return false;
                }

                if (roomItem.Coordinate != user.Coordinate)
                {
                    this.RoomInstance.GetGameMap().TeleportToItem(user, roomItem);
                }
            }
            else if (this.Items.Count == 1)
            {
                this.RoomInstance.GetGameMap().TeleportToItem(user, Enumerable.First<Item>(this.Items));
            }

            user.ApplyEffect(user.CurrentEffect, true);
            if (user.FreezeEndCounter <= 0)
            {
                user.Freeze = false;
            }

            return false;
        }

        public override void Handle(RoomUser user, Item item)
        {
            if (this.Items.Count == 0 || user == null)
            {
                return;
            }

            user.ApplyEffect(4, true);
            user.Freeze = true;

            base.Handle(user, item);
        }

        public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

        public void LoadFromDatabase(DataRow row)
        {
            int delay;
            if (int.TryParse(row["delay"].ToString(), out delay))
                this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out delay))
                this.Delay = delay + 1;

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == null || triggerItems == "")
                return;

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if (!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
