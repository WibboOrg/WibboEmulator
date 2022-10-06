namespace WibboEmulator.Games.Rooms.Trading;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.Items;

public class Trade
{
    private readonly TradeUser[] _users;
    private int _tradeStage;
    private readonly int _roomId;
    private readonly int _oneId;
    private readonly int _twoId;

    public bool AllUsersAccepted
    {
        get
        {
            foreach (var tradeUser in this._users)
            {
                if (tradeUser != null && !tradeUser.HasAccepted)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public Trade(int UserOneId, int UserTwoId, int RoomId)
    {
        this._oneId = UserOneId;
        this._twoId = UserTwoId;
        this._users = new TradeUser[2];
        this._users[0] = new TradeUser(UserOneId, RoomId);
        this._users[1] = new TradeUser(UserTwoId, RoomId);
        this._tradeStage = 1;
        this._roomId = RoomId;

        this.SendMessageToUsers(new TradingStartComposer(UserOneId, UserTwoId));

        foreach (var tradeUser in this._users)
        {
            if (!tradeUser.GetRoomUser().ContainStatus("trd"))
            {
                tradeUser.GetRoomUser().SetStatus("trd", "");
                tradeUser.GetRoomUser().UpdateNeeded = true;
            }
        }
    }

    public bool ContainsUser(int Id)
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser.UserId == Id)
            {
                return true;
            }
        }
        return false;
    }

    public TradeUser GetTradeUser(int Id)
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser.UserId == Id)
            {
                return tradeUser;
            }
        }

        return null;
    }

    public void OfferItem(int UserId, Item Item, bool UpdateWindows = true)
    {
        var tradeUser = this.GetTradeUser(UserId);
        if (tradeUser == null || Item == null || !Item.GetBaseItem().AllowTrade || tradeUser.HasAccepted || this._tradeStage != 1)
        {
            return;
        }

        this.ClearAccepted();

        if (!tradeUser.OfferedItems.Contains(Item))
        {
            tradeUser.OfferedItems.Add(Item);
        }

        if (UpdateWindows)
        {
            this.UpdateTradeWindow();
        }
    }

    public void TakeBackItem(int UserId, Item Item)
    {
        var tradeUser = this.GetTradeUser(UserId);
        if (tradeUser == null || Item == null || tradeUser.HasAccepted || this._tradeStage != 1)
        {
            return;
        }

        this.ClearAccepted();
        tradeUser.OfferedItems.Remove(Item);
        this.UpdateTradeWindow();
    }

    public void Accept(int UserId)
    {
        var tradeUser = this.GetTradeUser(UserId);
        if (tradeUser == null || this._tradeStage != 1)
        {
            return;
        }

        tradeUser.HasAccepted = true;
        this.SendMessageToUsers(new TradingAcceptComposer(UserId, 1));

        if (!this.AllUsersAccepted)
        {
            return;
        }

        this.SendMessageToUsers(new TradingCompleteComposer());
        this._tradeStage++;
        this.ClearAccepted();
    }

    public void Unaccept(int UserId)
    {
        var tradeUser = this.GetTradeUser(UserId);
        if (tradeUser == null || this._tradeStage != 1 || this.AllUsersAccepted)
        {
            return;
        }

        tradeUser.HasAccepted = false;
        this.SendMessageToUsers(new TradingAcceptComposer(UserId, 0));
    }

    public void CompleteTrade(int UserId)
    {
        var tradeUser = this.GetTradeUser(UserId);
        if (tradeUser == null || this._tradeStage != 2)
        {
            return;
        }

        tradeUser.HasAccepted = true;

        this.SendMessageToUsers(new TradingAcceptComposer(UserId, 1));

        if (!this.AllUsersAccepted)
        {
            return;
        }

        this._tradeStage = 999;
        this.EndTrade();
    }

    private void EndTrade()
    {
        try
        {
            if (this.DeliverItems())
            {
                this.SaveLogs();
            }

            this.CloseTradeClean();
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogThreadException(ex.ToString(), "Trade task");
        }
    }

    public void ClearAccepted()
    {
        foreach (var tradeUser in this._users)
        {
            tradeUser.HasAccepted = false;
        }
    }

    public void UpdateTradeWindow()
    {
        var tradeUserOne = this.GetTradeUser(this._oneId);
        var tradeUserTwo = this.GetTradeUser(this._twoId);

        if (tradeUserOne.GetClient() == null || tradeUserTwo.GetClient() == null)
        {
            return;
        }

        if (tradeUserOne.GetClient().GetUser() == null || tradeUserTwo.GetClient().GetUser() == null)
        {
            return;
        }

        this.SendMessageToUsers(new TradingUpdateComposer(tradeUserOne.UserId, tradeUserOne.OfferedItems, tradeUserTwo.UserId, tradeUserTwo.OfferedItems));
    }

    public bool DeliverItems()
    {
        var tradeUserOne = this.GetTradeUser(this._oneId);
        var tradeUserTwo = this.GetTradeUser(this._twoId);

        if (tradeUserOne.GetClient() == null || tradeUserTwo.GetClient() == null)
        {
            return false;
        }

        if (tradeUserOne.GetClient().GetUser() == null || tradeUserTwo.GetClient().GetUser() == null)
        {
            return false;
        }

        var userOneItems = tradeUserOne.OfferedItems;
        var userTwoItems = tradeUserTwo.OfferedItems;

        foreach (var userItem in userOneItems)
        {
            if (tradeUserOne.GetClient().GetUser().GetInventoryComponent().GetItem(userItem.Id) == null)
            {
                tradeUserOne.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserOne.GetClient().Langue));
                tradeUserTwo.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserTwo.GetClient().Langue));
                return false;
            }
        }

        foreach (var userItem in userTwoItems)
        {
            if (tradeUserTwo.GetClient().GetUser().GetInventoryComponent().GetItem(userItem.Id) == null)
            {
                tradeUserOne.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserOne.GetClient().Langue));
                tradeUserTwo.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserTwo.GetClient().Langue));
                return false;
            }
        }

        foreach (var userItem in userOneItems)
        {
            tradeUserOne.GetClient().GetUser().GetInventoryComponent().RemoveItem(userItem.Id);
            tradeUserTwo.GetClient().GetUser().GetInventoryComponent().AddItem(userItem);
        }

        foreach (var userItem in userTwoItems)
        {
            tradeUserTwo.GetClient().GetUser().GetInventoryComponent().RemoveItem(userItem.Id);
            tradeUserOne.GetClient().GetUser().GetInventoryComponent().AddItem(userItem);
        }

        tradeUserTwo.GetClient().SendPacket(new FurniListNotificationComposer(userOneItems, 1));
        tradeUserOne.GetClient().SendPacket(new FurniListNotificationComposer(userTwoItems, 1));

        tradeUserOne.GetClient().SendPacket(new FurniListUpdateComposer());
        tradeUserTwo.GetClient().SendPacket(new FurniListUpdateComposer());

        return true;
    }

    private void SaveLogs()
    {
        var ItemsOneCounter = new Dictionary<string, int>();
        var ItemsTwoCounter = new Dictionary<string, int>();

        foreach (var userItem in this.GetTradeUser(this._oneId).OfferedItems)
        {
            if (!ItemsOneCounter.ContainsKey(userItem.GetBaseItem().ItemName))
            {
                ItemsOneCounter.Add(userItem.GetBaseItem().ItemName, 1);
            }
            else
            {
                ItemsOneCounter[userItem.GetBaseItem().ItemName]++;
            }
        }

        foreach (var userItem in this.GetTradeUser(this._twoId).OfferedItems)
        {
            if (!ItemsTwoCounter.ContainsKey(userItem.GetBaseItem().ItemName))
            {
                ItemsTwoCounter.Add(userItem.GetBaseItem().ItemName, 1);
            }
            else
            {
                ItemsTwoCounter[userItem.GetBaseItem().ItemName]++;
            }
        }

        var LogsOneString = "";
        foreach (var Logs in ItemsOneCounter)
        {
            LogsOneString += $"{Logs.Key} ({Logs.Value}),";
        }

        LogsOneString = LogsOneString.TrimEnd(',');

        var LogsTwoString = "";
        foreach (var Logs in ItemsTwoCounter)
        {
            LogsTwoString += $"{Logs.Key} ({Logs.Value}),";
        }

        LogsTwoString = LogsTwoString.TrimEnd(',');

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        LogTradeDao.Insert(dbClient, this._oneId, this._twoId, LogsOneString, LogsTwoString, this._roomId);
    }

    public void CloseTradeClean()
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser != null && tradeUser.GetRoomUser() != null)
            {
                tradeUser.GetRoomUser().RemoveStatus("trd");
                tradeUser.GetRoomUser().UpdateNeeded = true;
            }
        }

        this.SendMessageToUsers(new TradingFinishComposer());

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this._roomId, out var room))
        {
            room.ActiveTrades.Remove(this);
        }
    }

    public void CloseTrade(int userId)
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser != null && tradeUser.GetRoomUser() != null)
            {
                tradeUser.GetRoomUser().RemoveStatus("trd");
                tradeUser.GetRoomUser().UpdateNeeded = true;
            }
        }

        this.SendMessageToUsers(new TradingClosedComposer(userId));
    }

    public void SendMessageToUsers(ServerPacket message)
    {
        if (this._users == null)
        {
            return;
        }

        foreach (var tradeUser in this._users)
        {
            if (tradeUser != null && tradeUser.GetClient() != null)
            {
                tradeUser.GetClient().SendPacket(message);
            }
        }
    }
}
