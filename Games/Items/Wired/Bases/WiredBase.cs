namespace WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public class WiredBase
{
    internal bool StuffTypeSelectionEnabled { get; set; }
    internal int FurniLimit { get; set; }
    internal List<int> StuffIds { get; set; }
    internal int StuffTypeId { get; set; }
    internal int Id { get; set; }
    internal string StringParam { get; set; }
    internal List<int> IntParams { get; set; }
    internal int StuffTypeSelectionCode { get; set; }
    internal int Type { get; set; }
    internal List<int> Conflicting { get; set; }
    internal int Delay { get; set; }

    internal Item ItemInstance { get; set; }
    internal Room RoomInstance { get; set; }
    internal List<Item> Items { get; set; }

    internal bool IsStaff { get; set; }
    internal bool IsGod { get; set; }
    internal bool IsDisposed { get; set; }

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
        this.StringParam = stringParam.Length <= 255 ? stringParam : stringParam[..255];
        this.StuffIds = stuffIds;
        this.StuffTypeSelectionCode = selectionCode;
        this.Delay = delay;
        this.IsStaff = isStaff;
        this.IsGod = isGod;
    }

    public virtual void OnTrigger(GameClient session)
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

        if (this.StuffIds != null)
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

    public bool Disposed() => this.IsDisposed;

    public virtual void LoadItems(bool inDatabase = false)
    {
        this.Items = this.GetItems();

        this.StuffIds.Clear();
        foreach (var item in this.Items.ToList())
        {
            this.StuffIds.Add(item.Id);
        }
    }

    internal List<Item> GetItems()
    {
        var listItem = new List<Item>();
        foreach (var itemId in this.StuffIds)
        {
            var item = this.RoomInstance.RoomItemHandling.GetItem(itemId);
            if (item != null && item.GetBaseItem().Type == 's')
            {
                listItem.Add(item);
            }

            if (listItem.Count >= this.FurniLimit)
            {
                break;
            }
        }

        return listItem;
    }
}
