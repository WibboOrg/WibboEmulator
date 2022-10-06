namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class PositionReset : WiredActionBase, IWired, IWiredEffect
{
    private readonly Dictionary<int, ItemsPosReset> _itemsData;

    public PositionReset(Item item, Room room) : base(item, room, (int)WiredActionType.SET_FURNI_STATE)
    {
        this._itemsData = new Dictionary<int, ItemsPosReset>();

        this.IntParams.Add(0);
        this.IntParams.Add(0);
        this.IntParams.Add(0);
    }

    public override void LoadItems(bool inDatabase = false)
    {
        base.LoadItems();

        if (inDatabase)
        {
            return;
        }

        this._itemsData.Clear();

        foreach (var roomItem in this.Items.ToList())
        {
            if (!this._itemsData.ContainsKey(roomItem.Id))
            {
                this._itemsData.Add(roomItem.Id, new ItemsPosReset(roomItem.Id, roomItem.X, roomItem.Y, roomItem.Z, roomItem.Rotation, roomItem.ExtraData));
            }
            else
            {
                _ = this._itemsData.Remove(roomItem.Id);
                this._itemsData.Add(roomItem.Id, new ItemsPosReset(roomItem.Id, roomItem.X, roomItem.Y, roomItem.Z, roomItem.Rotation, roomItem.ExtraData));
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
        var state = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;
        var direction = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0) == 1;
        var position = ((this.IntParams.Count > 2) ? this.IntParams[2] : 0) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            if (roomItem == null)
            {
                continue;
            }

            if (!this._itemsData.TryGetValue(roomItem.Id, out var itemPosReset))
            {
                continue;
            }

            if (state)
            {
                if (itemPosReset.ExtraData != "Null")
                {
                    if (roomItem.ExtraData != itemPosReset.ExtraData)
                    {
                        roomItem.ExtraData = itemPosReset.ExtraData;
                        roomItem.UpdateState();
                        this.RoomInstance.GetGameMap().UpdateMapForItem(roomItem);
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
        var triggerItems = "";

        foreach (var roomItem in this._itemsData.Values)
        {
            triggerItems += roomItem.Id + ":" + roomItem.X + ":" + roomItem.Y + ":" + roomItem.Z + ":" + roomItem.Rot + ":" + roomItem.ExtraData + ";";
        }

        triggerItems = triggerItems.TrimEnd(';');

        var state = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var direction = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;
        var position = (this.IntParams.Count > 2) ? this.IntParams[2] : 0;

        var triggerData2 = state + ";" + direction + ";" + position;

        ItemWiredDao.Delete(dbClient, this.ItemInstance.Id);
        ItemWiredDao.Insert(dbClient, this.ItemInstance.Id, "", triggerData2, false, triggerItems, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out delay))
        {
            this.Delay = delay;
        }

        var triggerData2 = row["trigger_data_2"].ToString();

        if (triggerData2 != null && triggerData2.Length == 5)
        {
            var dataSplit = triggerData2.Split(';');

            if (int.TryParse(dataSplit[0], out var state))
            {
                this.IntParams.Add(state);
            }

            if (int.TryParse(dataSplit[1], out var direction))
            {
                this.IntParams.Add(direction);
            }

            if (int.TryParse(dataSplit[2], out var position))
            {
                this.IntParams.Add(position);
            }
        }

        var triggerItems = row["triggers_item"].ToString();

        if (triggerItems is null or "")
        {
            return;
        }

        foreach (var item in triggerItems.Split(';'))
        {
            var itemData = item.Split(':');
            if (itemData.Length != 6)
            {
                continue;
            }

            if (!int.TryParse(itemData[0], out var id))
            {
                continue;
            }

            if (!this.StuffIds.Contains(id))
            {
                this.StuffIds.Add(id);
            }

            this._itemsData.Add(Convert.ToInt32(itemData[0]), new ItemsPosReset(Convert.ToInt32(itemData[0]), Convert.ToInt32(itemData[1]), Convert.ToInt32(itemData[2]), Convert.ToDouble(itemData[3]), Convert.ToInt32(itemData[4]), itemData[5]));
        }
    }
}
