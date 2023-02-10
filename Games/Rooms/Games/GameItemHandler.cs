namespace WibboEmulator.Games.Rooms.Games;
using System.Drawing;
using WibboEmulator.Games.Items;

public class GameItemHandler
{
    private Dictionary<int, Item> _banzaiTeleports;
    private Dictionary<int, Item> _banzaiPyramids;
    private readonly Dictionary<Point, List<Item>> _groupGate;
    private readonly Dictionary<int, Item> _banzaiBlobs;
    private Room _roomInstance;
    private Item _exitTeleport;

    public GameItemHandler(Room room)
    {
        this._roomInstance = room;
        this._banzaiPyramids = new Dictionary<int, Item>();
        this._banzaiTeleports = new Dictionary<int, Item>();
        this._groupGate = new Dictionary<Point, List<Item>>();
        this._banzaiBlobs = new Dictionary<int, Item>();
    }

    public void OnCycle() => this.CyclePyramids();

    private void CyclePyramids()
    {
        if (this._banzaiPyramids == null)
        {
            return;
        }

        foreach (var roomItem in this._banzaiPyramids.Values.ToList())
        {
            if (roomItem.InteractionCountHelper == 0 && roomItem.ExtraData == "1")
            {
                roomItem.InteractionCountHelper = 1;
            }
            if (string.IsNullOrEmpty(roomItem.ExtraData))
            {
                roomItem.ExtraData = "0";
            }

            if (WibboEnvironment.GetRandomNumber(0, 30) == 15)
            {
                if (roomItem.ExtraData == "0")
                {
                    roomItem.ExtraData = "1";
                    roomItem.UpdateState();
                    this._roomInstance.GameMap.UpdateMapForItem(roomItem);
                }
                else if (this._roomInstance.GameMap.CanStackItem(roomItem.X, roomItem.Y))
                {
                    roomItem.ExtraData = "0";
                    roomItem.UpdateState();
                    this._roomInstance.GameMap.UpdateMapForItem(roomItem);
                }
            }
        }
    }


    public void AddPyramid(Item item, int itemID)
    {
        if (this._banzaiPyramids.ContainsKey(itemID))
        {
            this._banzaiPyramids[itemID] = item;
        }
        else
        {
            this._banzaiPyramids.Add(itemID, item);
        }
    }

    public void RemovePyramid(int itemID) => this._banzaiPyramids.Remove(itemID);

    public void RemoveBlob(int itemID) => this._banzaiBlobs.Remove(itemID);

    public Item GetExitTeleport() => this._exitTeleport;

    public void AddExitTeleport(Item item) => this._exitTeleport = item;

    public void RemoveExitTeleport(Item item)
    {
        var exitTeleport = this._exitTeleport;
        if (exitTeleport != null && item.Id == exitTeleport.Id)
        {
            this._exitTeleport = null;
        }
    }

    public void AddBlob(Item item, int itemID)
    {
        if (this._banzaiBlobs.ContainsKey(itemID))
        {
            this._banzaiBlobs[itemID] = item;
        }
        else
        {
            this._banzaiBlobs.Add(itemID, item);
        }
    }

    public void OnWalkableBanzaiBlob(RoomUser user, Item item)
    {
        if (item.ExtraData == "1")
        {
            return;
        }

        this._roomInstance.
        GameManager.AddPointToTeam(user.Team, user);
        item.ExtraData = "1";
        item.UpdateState();
    }

    public void OnWalkableBanzaiBlob2(RoomUser user, Item item)
    {
        if (item.ExtraData == "1")
        {
            return;
        }

        this._roomInstance.
        GameManager.AddPointToTeam(user.Team, 5, user);
        item.ExtraData = "1";
        item.UpdateState();
    }

    public void ResetAllBlob()
    {
        foreach (var blob in this._banzaiBlobs.Values)
        {
            if (blob.ExtraData == "0")
            {
                continue;
            }

            blob.ExtraData = "0";
            blob.UpdateState();
        }
    }

    public void AddGroupGate(Item item)
    {
        if (this._groupGate.TryGetValue(item.Coordinate, out var value))
        {
            value.Add(item);
        }
        else
        {
            this._groupGate.Add(item.Coordinate, new List<Item>() { item });
        }
    }

    public void RemoveGroupGate(Item item)
    {
        if (!this._groupGate.ContainsKey(item.Coordinate))
        {
            return;
        }
        _ = this._groupGate[item.Coordinate].Remove(item);
        if (this._groupGate.Count == 0)
        {
            _ = this._groupGate.Remove(item.Coordinate);
        }
    }

    public void AddTeleport(Item item, int itemID)
    {
        if (this._banzaiTeleports.ContainsKey(itemID))
        {
            //this.banzaiTeleports.Inner[itemID] = item;
            _ = this._banzaiTeleports.Remove(itemID);
            this._banzaiTeleports.Add(itemID, item);
        }
        else
        {
            this._banzaiTeleports.Add(itemID, item);
        }
    }

    public void RemoveTeleport(int itemID) => this._banzaiTeleports.Remove(itemID);

    public bool CheckGroupGate(RoomUser user, Point coordinate)
    {
        if (this._groupGate == null)
        {
            return false;
        }

        if (!this._groupGate.ContainsKey(coordinate))
        {
            return false;
        }

        if (this._groupGate[coordinate].Count == 0)
        {
            return false;
        }

        var item = this._groupGate[coordinate].FirstOrDefault();

        if (item == null)
        {
            return false;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(item.GroupId, out var group))
        {
            return true;
        }

        if (user == null)
        {
            return false;
        }

        if (user.IsBot)
        {
            return false;
        }

        if (user.Client == null)
        {
            return false;
        }

        if (user.Client.User == null)
        {
            return false;
        }

        if (user.Client.User.Rank > 5)
        {
            return false;
        }

        if (user.Client.User.MyGroups == null)
        {
            return true;
        }

        if (user.Client.User.MyGroups.Contains(group.Id))
        {
            return false;
        }

        return true;
    }

    public void OnTeleportRoomUserEnter(RoomUser user, Item item)
    {
        var banzaiTeleports2 = this._banzaiTeleports.Values.Where(p => p.Id != item.Id);

        var count = banzaiTeleports2.Count();

        if (count == 0)
        {
            return;
        }

        var countID = WibboEnvironment.GetRandomNumber(0, count - 1);
        var banzaiItem2 = banzaiTeleports2.ElementAt(countID);

        if (banzaiItem2 == null)
        {
            return;
        }

        if (banzaiItem2.InteractingUser != 0)
        {
            return;
        }

        user.IsWalking = false;
        user.CanWalk = false;
        banzaiItem2.InteractingUser = user.UserId;
        banzaiItem2.ReqUpdate(2);

        item.ExtraData = "1";
        item.UpdateState(false);
        item.ReqUpdate(2);
    }

    public void Destroy()
    {
        this._banzaiTeleports?.Clear();

        this._banzaiPyramids?.Clear();

        this._banzaiPyramids = null;
        this._banzaiTeleports = null;
        this._roomInstance = null;
    }
}
