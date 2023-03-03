namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class FurniStatePosMatch : WiredConditionBase, IWiredCondition, IWired
{
    private readonly Dictionary<int, ItemsPosReset> _itemsData;

    public FurniStatePosMatch(Item item, Room room) : base(item, room, (int)WiredConditionType.STATES_MATCH)
    {
        this._itemsData = new Dictionary<int, ItemsPosReset>();

        this.IntParams.Add(0);
        this.IntParams.Add(0);
        this.IntParams.Add(0);
        this.IntParams.Add(0);
        this.IntParams.Add(1);
    }
    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (this.Items.Count == 0)
        {
            return false;
        }

        var state = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;
        var direction = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0) == 1;
        var position = ((this.IntParams.Count > 2) ? this.IntParams[2] : 0) == 1;
        var height = ((this.IntParams.Count > 3) ? this.IntParams[3] : 0) == 1;
        var requireAll = ((this.IntParams.Count > 4) ? this.IntParams[4] : 0) == 1;

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
        var height = (this.IntParams.Count > 3) ? this.IntParams[3] : 0;
        var requireAll = (this.IntParams.Count > 4) ? this.IntParams[4] : 1;

        var triggerData2 = string.Join(";", new int[] { state, direction, position, height, requireAll });

        ItemWiredDao.Delete(dbClient, this.ItemInstance.Id);
        ItemWiredDao.Insert(dbClient, this.ItemInstance.Id, "", triggerData2, false, triggerItems, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        var triggerData2 = row["trigger_data_2"].ToString();

        if (triggerData2 != null && triggerData2.Contains(';'))
        {
            foreach (var index in triggerData2.Split(';'))
            {
                if (int.TryParse(index, out var state))
                {
                    this.IntParams.Add(state);
                }
            }
        }

        if (this.IntParams.Count <= 3)
        {
            this.IntParams.Add(0);
        }

        if (this.IntParams.Count <= 4)
        {
            this.IntParams.Add(1);
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
