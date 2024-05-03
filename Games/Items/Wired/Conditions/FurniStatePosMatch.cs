namespace WibboEmulator.Games.Items.Wired.Conditions;

using System;
using WibboEmulator.Database.Daos.Item;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class FurniStatePosMatch : WiredConditionBase, IWiredCondition, IWired
{
    private readonly Dictionary<int, ItemsPosReset> _itemsData;

    public FurniStatePosMatch(Item item, Room room) : base(item, room, (int)WiredConditionType.STATES_MATCH)
    {
        this._itemsData = [];

        this.DefaultIntParams(0, 0, 0, 0, 1);
    }
    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (this.Items.Count == 0)
        {
            return false;
        }

        var state = this.GetIntParam(0) == 1;
        var direction = this.GetIntParam(1) == 1;
        var position = this.GetIntParam(2) == 1;
        var height = this.GetIntParam(3) == 1;
        var requireAll = this.GetIntParam(4) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            if (!this._itemsData.TryGetValue(roomItem.Id, out var itemPosReset))
            {
                continue;
            }

            var isValide = true;

            if (state)
            {
                if (itemPosReset.ExtraData != "Null")
                {
                    if (!(roomItem.ExtraData == "" && itemPosReset.ExtraData == "0") && !(roomItem.ExtraData == "0" && itemPosReset.ExtraData == ""))
                    {
                        if (roomItem.ExtraData != itemPosReset.ExtraData)
                        {
                            isValide = false;
                        }
                    }
                }
            }

            if (direction)
            {
                if (itemPosReset.Rot != roomItem.Rotation)
                {
                    isValide = false;
                }
            }

            if (position)
            {
                if (itemPosReset.X != roomItem.X || itemPosReset.Y != roomItem.Y)
                {
                    isValide = false;
                }
            }

            if (height)
            {
                if (itemPosReset.Z != roomItem.Z)
                {
                    isValide = false;
                }
            }

            if (!requireAll && isValide)
            {
                return true;
            }

            if (requireAll && !isValide)
            {
                return false;
            }
        }

        return requireAll;
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
        var height = this.GetIntParam(3);
        var requireAll = this.GetIntParam(4);

        var triggerData2 = string.Join(";", new int[] { state, direction, position, height, requireAll });

        ItemWiredDao.Replace(dbClient, this.Item.Id, string.Empty, triggerData2, false, triggerItems, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (wiredTriggerData2.Contains(';'))
        {
            var index = 0;
            foreach (var value in wiredTriggerData2.Split(';'))
            {
                if (int.TryParse(value, out var state))
                {
                    this.SetIntParam(index, state);
                }

                index++;
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
