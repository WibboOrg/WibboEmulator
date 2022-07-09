using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;

namespace Wibbo.Game.Items.Wired.Actions
{
    public class PositionReset : WiredActionBase, IWired, IWiredEffect
    {
        private readonly Dictionary<int, ItemsPosReset> ItemsData;

        public PositionReset(Item item, Room room) : base(item, room, (int)WiredActionType.SET_FURNI_STATE)
        {
            this.ItemsData = new Dictionary<int, ItemsPosReset>();

            this.IntParams.Add(0);
            this.IntParams.Add(0);
            this.IntParams.Add(0);
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems();

            if(inDatabase)
                return;

            this.ItemsData.Clear();

            foreach (Item roomItem in this.Items)
            {
                if (!this.ItemsData.ContainsKey(roomItem.Id))
                {
                    this.ItemsData.Add(roomItem.Id, new ItemsPosReset(roomItem.Id, roomItem.X, roomItem.Y, roomItem.Z, roomItem.Rotation, roomItem.ExtraData));
                }
                else
                {
                    this.ItemsData.Remove(roomItem.Id);
                    this.ItemsData.Add(roomItem.Id, new ItemsPosReset(roomItem.Id, roomItem.X, roomItem.Y, roomItem.Z, roomItem.Rotation, roomItem.ExtraData));
                }
            }
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();
            return false;
        }

        private void HandleItems()
        {
            bool state = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;
            bool direction = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0) == 1;
            bool position = ((this.IntParams.Count > 2) ? this.IntParams[2] : 0) == 1;

            foreach (Item roomItem in this.Items)
            {
                if(roomItem == null)
                    continue;
                    
                if(!this.ItemsData.TryGetValue(roomItem.Id, out ItemsPosReset itemPosReset))
                    continue;

                if (state)
                {
                    if (itemPosReset.ExtraData != "Null")
                    {
                        if (roomItem.ExtraData != itemPosReset.ExtraData)
                        {
                            roomItem.ExtraData = itemPosReset.ExtraData;
                            roomItem.UpdateState();
                            roomItem.GetRoom().GetGameMap().UpdateMapForItem(roomItem);
                        }
                    }
                }

                if (direction)
                {
                    if (itemPosReset.Rot != roomItem.Rotation)
                    {
                        this.RoomInstance.GetRoomItemHandler().RotReset(roomItem, itemPosReset.Rot);
                    }
                }

                if (position)
                {
                    if (itemPosReset.X != roomItem.X || itemPosReset.Y != roomItem.Y || itemPosReset.Z != roomItem.Z)
                    {
                        this.RoomInstance.GetRoomItemHandler().PositionReset(roomItem, itemPosReset.X, itemPosReset.Y, itemPosReset.Z);
                    }
                }
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggerItems = "";

            foreach (ItemsPosReset roomItem in this.ItemsData.Values)
            {
                triggerItems += roomItem.Id + ":" + roomItem.X + ":" + roomItem.Y + ":" + roomItem.Z + ":" + roomItem.Rot + ":" + roomItem.ExtraData + ";";
            }

            triggerItems = triggerItems.TrimEnd(';');

            int state = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int direction = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);
            int position = ((this.IntParams.Count > 2) ? this.IntParams[2] : 0);

            string triggerData2 = state + ";" + direction + ";" + position;

            ItemWiredDao.Delete(dbClient, this.ItemInstance.Id);
            ItemWiredDao.Insert(dbClient, this.ItemInstance.Id, "", triggerData2, false, triggerItems, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            int delay;
            if (int.TryParse(row["delay"].ToString(), out delay))
                this.Delay = delay;

            if (int.TryParse(row["trigger_data"].ToString(), out delay))
                this.Delay = delay;

            string triggerData2 = row["trigger_data_2"].ToString();

            if (triggerData2.Length == 5)
            {
                string[] dataSplit = triggerData2.Split(';');

                if (int.TryParse(dataSplit[0], out int state))
                    this.IntParams.Add(state);
                if (int.TryParse(dataSplit[1], out int direction))
                    this.IntParams.Add(direction);
                if (int.TryParse(dataSplit[2], out int position))
                    this.IntParams.Add(position);
            }

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
            {
                return;
            }

            foreach (string item in triggerItems.Split(';'))
            {
                string[] itemData = item.Split(':');
                if (itemData.Length != 6)
                {
                    continue;
                }

                if (!int.TryParse(itemData[0], out int id))
                    continue;

                if(!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);

                this.ItemsData.Add(Convert.ToInt32(itemData[0]), new ItemsPosReset(Convert.ToInt32(itemData[0]), Convert.ToInt32(itemData[1]), Convert.ToInt32(itemData[2]), Convert.ToDouble(itemData[3]), Convert.ToInt32(itemData[4]), itemData[5]));
            }
        }
    }
}
