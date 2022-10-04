using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired
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

        internal Item ItemInstance;
        internal Room RoomInstance;
        internal List<Item> Items;

        internal bool IsStaff;
        internal bool IsGod;
        internal bool IsDisposed;

        internal WiredBase(Item item, Room room, int type)
        {
            this.Items = new List<Item>();
            this.ItemInstance = item;
            this.RoomInstance = room;
            this.Type = type;

            this.Id = item.Id;
            this.StuffTypeId = item.GetBaseItem().SpriteId;

            this.StuffTypeSelectionEnabled = false;
            this.FurniLimit = 10;
            this.StuffIds = new List<int>();
            this.StringParam = "";
            this.IntParams = new List<int>();
            this.StuffTypeSelectionCode = 0;
            this.Conflicting = new List<int>();
            this.Delay = 0;

            this.IsStaff = false;
            this.IsGod = false;
            this.IsDisposed = false;
        }

        public void Init(List<int> intParams, string stringParam, List<int> stuffIds, int selectionCode, int delay, bool isStaff, bool isGod)
        {
            this.IntParams = intParams;
            this.StringParam = (stringParam.Length <= 255) ? stringParam : stringParam.Substring(0, 255);
            this.StuffIds = stuffIds;
            this.StuffTypeSelectionCode = selectionCode;
            this.Delay = delay;
            this.IsStaff = isStaff;
            this.IsGod = isGod;
        }

        public virtual void OnTrigger(GameClient Session)
        {
            
        }

        public virtual void Dispose()
        {
            this.IsDisposed = true;

            if (this.Items != null)
            {
                this.Items.Clear();
                this.Items = null;
            }

            if(this.StuffIds != null)
            {
                this.StuffIds.Clear();
                this.StuffIds = null;
            }

            if (this.IntParams != null)
            {
                this.IntParams.Clear();
                this.IntParams = null;
            }

            if (this.Conflicting != null)
            {
                this.Conflicting.Clear();
                this.Conflicting = null;
            }
        }

        public bool Disposed()
        {
            return this.IsDisposed;
        }

        public virtual void LoadItems(bool inDatabase = false)
        {
            this.Items = this.GetItems();

            this.StuffIds.Clear();
            foreach (Item item in this.Items.ToList())
            {
                this.StuffIds.Add(item.Id);
            }
        }

        internal List<Item> GetItems()
        {
            List<Item> listItem = new List<Item>();
            foreach (int itemId in this.StuffIds)
            {
                Item item = this.RoomInstance.GetRoomItemHandler().GetItem(itemId);
                if (item != null && item.GetBaseItem().Type == 's')
                {
                    listItem.Add(item);
                }

                if (listItem.Count >= this.FurniLimit)
                    break;
            }

            return listItem;
        }
    }
}
