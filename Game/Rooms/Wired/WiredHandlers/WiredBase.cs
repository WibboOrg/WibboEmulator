using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class WiredBase
    {
        internal bool StuffTypeSelectionEnabled;
        internal int FurniLimit;
        internal List<int> StuffIds;
        internal int StuffTypeId;
        internal int Id;
        internal string StringParam;
        internal List<int> IntParams;
        internal int StuffTypeSelectionCode;
        internal int Type;
        internal List<int> Conflicting;
        internal int Delay;

        internal bool isDisposed;
        internal WiredBase()
        {
            this.StuffTypeSelectionEnabled = false;
            this.FurniLimit = 10;
            this.StuffIds = new List<int>();
            this.StuffTypeId = 0;
            this.Id = 0;
            this.StringParam = "";
            this.IntParams = new List<int>();
            this.StuffTypeSelectionCode = 0;
            this.Type = 0;
            this.Conflicting = new List<int>();
            this.Delay = 0;
        }
        internal virtual void SendWiredPacket(Client Session)
        {
            
        }

        public virtual void Dispose()
        {
            this.isDisposed = true;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }

        internal List<Item> GetItems(List<int> stuffIds, Room room)
        {
            List<Item> listItem = new List<Item>();
            foreach (int itemId in stuffIds)
            {
                Item item = room.GetRoomItemHandler().GetItem(itemId);
                if (item != null && item.GetBaseItem().Type == 's')
                {
                    listItem.Add(item);
                }
            }

            return listItem;
        }
    }
}
