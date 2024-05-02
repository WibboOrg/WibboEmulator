namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Daos.Item;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class PositionReset : WiredActionBase, IWired, IWiredEffect
{
    private readonly Dictionary<int, ItemsPosReset> _itemsData;

    public PositionReset(Item item, Room room) : base(item, room, (int)WiredActionType.SET_FURNI_STATE)
    {
        this._itemsData = new Dictionary<int, ItemsPosReset>();

        this.DefaultIntParams(new int[] { 0, 0, 0 });
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
            var isDice = roomItem.ItemData.InteractionType == InteractionType.DICE;

            if (!this._itemsData.ContainsKey(roomItem.Id))
            {
                this._itemsData.Add(roomItem.Id, new ItemsPosReset(roomItem.Id, roomItem.X, roomItem.Y, roomItem.Z, roomItem.Rotation, isDice ? "0" : roomItem.ExtraData));
            }
            else
            {
                _ = this._itemsData.Remove(roomItem.Id);
                this._itemsData.Add(roomItem.Id, new ItemsPosReset(roomItem.Id, roomItem.X, roomItem.Y, roomItem.Z, roomItem.Rotation, isDice ? "0" : roomItem.ExtraData));
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
        var disableAnimation = this.Room.WiredHandler.DisableAnimate(this.Item.Coordinate);

        var state = this.GetIntParam(0) == 1;
        var direction = this.GetIntParam(1) == 1;
        var position = this.GetIntParam(2) == 1;

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
                        this.Room.GameMap.UpdateMapForItem(roomItem);
                    }
                }
            }

            if (direction)
            {
                if (itemPosReset.Rot != roomItem.Rotation)
                {
                    roomItem.Rotation = itemPosReset.Rot;

                    roomItem.UpdateState(false);
                }
            }

            if (position)
            {
                if (itemPosReset.X != roomItem.X || itemPosReset.Y != roomItem.Y || itemPosReset.Z != roomItem.Z)
                {
                    this.Room.RoomItemHandling.PositionReset(roomItem, itemPosReset.X, itemPosReset.Y, itemPosReset.Z, disableAnimation);
                }
            }
        }
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var triggerItems = "";

        foreach (var roomItem in this._itemsData.Values)
        {
            triggerItems += roomItem.Id + ":" + roomItem.X + ":" + roomItem.Y + ":" + roomItem.Z + ":" + roomItem.Rot + ":" + roomItem.ExtraData + ";";
        }

        triggerItems = triggerItems.TrimEnd(';');

        var state = this.GetIntParam(0);
        var direction = this.GetIntParam(1);
        var position = this.GetIntParam(2);

        var triggerData2 = state + ";" + direction + ";" + position;

        ItemWiredDao.Replace(dbClient, this.Item.Id, "", triggerData2, false, triggerItems, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.Delay = delay;
        }

        if (wiredTriggerData2.Length == 5)
        {
            var dataSplit = wiredTriggerData2.Split(';');

            if (dataSplit.Length == 3)
            {
                if (int.TryParse(dataSplit[0], out var state))
                {
                    this.SetIntParam(0, state);
                }

                if (int.TryParse(dataSplit[1], out var direction))
                {
                    this.SetIntParam(1, direction);
                }

                if (int.TryParse(dataSplit[2], out var position))
                {
                    this.SetIntParam(2, position);
                }
            }
        }

        if (wiredTriggersItem is "")
        {
            return;
        }

        foreach (var item in wiredTriggersItem.Split(';'))
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
