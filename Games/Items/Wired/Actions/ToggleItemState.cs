using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Actions
{
    public class ToggleItemState : WiredActionBase, IWired, IWiredEffect
    {
        public ToggleItemState(Item item, Room room) : base(item, room, (int)WiredActionType.TOGGLE_FURNI_STATE)
        {
            this.IntParams.Add(0);
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            bool isReverse = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

            foreach (Item roomItem in this.Items)
            {
                if (roomItem != null)
                {
                    if (user != null && user.GetClient() != null)
                    {
                        roomItem.Interactor.OnTrigger(user.GetClient(), roomItem, 0, true, isReverse);
                    }
                    else
                    {
                        roomItem.Interactor.OnTrigger(null, roomItem, 0, true, isReverse);
                    }

                }
            }

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int reverse = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, reverse.ToString(), string.Empty, false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            int delay;
            if (int.TryParse(row["delay"].ToString(), out delay))
	            this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out delay))
                this.Delay = delay;

            if (int.TryParse(row["trigger_data_2"].ToString(), out int reverse))
                this.IntParams.Add(reverse);

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
                return;

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if(!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
