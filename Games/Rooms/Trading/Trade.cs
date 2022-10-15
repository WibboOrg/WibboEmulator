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

    public Trade(int userOneId, int userTwoId, int roomId)
    {
        this._oneId = userOneId;
        this._twoId = userTwoId;
        this._users = new TradeUser[2];
        this._users[0] = new TradeUser(userOneId, roomId);
        this._users[1] = new TradeUser(userTwoId, roomId);
        this._tradeStage = 1;
        this._roomId = roomId;

        this.SendMessageToUsers(new TradingStartComposer(userOneId, userTwoId));

        foreach (var tradeUser in this._users)
        {
            if (!tradeUser.GetRoomUser().ContainStatus("trd"))
            {
                tradeUser.GetRoomUser().SetStatus("trd", "");
                tradeUser.GetRoomUser().UpdateNeeded = true;
            }
        }
    }

    public bool ContainsUser(int id)
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser.UserId == id)
            {
                return true;
            }
        }
        return false;
    }

    public TradeUser GetTradeUser(int id)
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser.UserId == id)
            {
                return tradeUser;
            }
        }

        return null;
    }

    public void OfferItem(int userId, Item item, bool updateWindows = true)
    {
        var tradeUser = this.GetTradeUser(userId);
        if (tradeUser == null || item == null || !item.GetBaseItem().AllowTrade || tradeUser.HasAccepted || this._tradeStage != 1)
        {
            return;
        }

        this.ClearAccepted();

        if (!tradeUser.OfferedItems.Contains(item))
        {
            tradeUser.OfferedItems.Add(item);
        }

        if (updateWindows)
        {
            this.UpdateTradeWindow();
        }
    }

    public void TakeBackItem(int userId, Item item)
    {
        var tradeUser = this.GetTradeUser(userId);
        if (tradeUser == null || item == null || tradeUser.HasAccepted || this._tradeStage != 1)
        {
            return;
        }

        this.ClearAccepted();
        _ = tradeUser.OfferedItems.Remove(item);
        this.UpdateTradeWindow();
    }

    public void Accept(int userId)
    {
        var tradeUser = this.GetTradeUser(userId);
        if (tradeUser == null || this._tradeStage != 1)
        {
            return;
        }

        tradeUser.HasAccepted = true;
        this.SendMessageToUsers(new TradingAcceptComposer(userId, 1));

        if (!this.AllUsersAccepted)
        {
            return;
        }

        this.SendMessageToUsers(new TradingCompleteComposer());
        this._tradeStage++;
        this.ClearAccepted();
    }

    public void Unaccept(int userId)
    {
        var tradeUser = this.GetTradeUser(userId);
        if (tradeUser == null || this._tradeStage != 1 || this.AllUsersAccepted)
        {
            return;
        }

        tradeUser.HasAccepted = false;
        this.SendMessageToUsers(new TradingAcceptComposer(userId, 0));
    }

    public void CompleteTrade(int userId)
    {
        var tradeUser = this.GetTradeUser(userId);
        if (tradeUser == null || this._tradeStage != 2)
        {
            return;
        }

        tradeUser.HasAccepted = true;

        this.SendMessageToUsers(new TradingAcceptComposer(userId, 1));

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

        if (tradeUserOne.GetClient().User == null || tradeUserTwo.GetClient().User == null)
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

        if (tradeUserOne.GetClient().User == null || tradeUserTwo.GetClient().User == null)
        {
            return false;
        }

        var userOneItems = tradeUserOne.OfferedItems;
        var userTwoItems = tradeUserTwo.OfferedItems;

        foreach (var userItem in userOneItems)
        {
            if (tradeUserOne.GetClient().User.InventoryComponent.GetItem(userItem.Id) == null)
            {
                tradeUserOne.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserOne.GetClient().Langue));
                tradeUserTwo.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserTwo.GetClient().Langue));
                return false;
            }
        }

        foreach (var userItem in userTwoItems)
        {
            if (tradeUserTwo.GetClient().User.InventoryComponent.GetItem(userItem.Id) == null)
            {
                tradeUserOne.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserOne.GetClient().Langue));
                tradeUserTwo.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserTwo.GetClient().Langue));
                return false;
            }
        }

        foreach (var userItem in userOneItems)
        {
            tradeUserOne.GetClient().User.InventoryComponent.RemoveItem(userItem.Id);
            tradeUserTwo.GetClient().User.InventoryComponent.AddItem(userItem);
        }

        foreach (var userItem in userTwoItems)
        {
            tradeUserTwo.GetClient().User.InventoryComponent.RemoveItem(userItem.Id);
            tradeUserOne.GetClient().User.InventoryComponent.AddItem(userItem);
        }

        tradeUserTwo.GetClient().SendPacket(new FurniListNotificationComposer(userOneItems, 1));
        tradeUserOne.GetClient().SendPacket(new FurniListNotificationComposer(userTwoItems, 1));

        tradeUserOne.GetClient().SendPacket(new FurniListUpdateComposer());
        tradeUserTwo.GetClient().SendPacket(new FurniListUpdateComposer());

        return true;
    }

    private void SaveLogs()
    {
        var itemsOneCounter = new Dictionary<string, int>();
        var itemsTwoCounter = new Dictionary<string, int>();

        foreach (var userItem in this.GetTradeUser(this._oneId).OfferedItems)
        {
            if (!itemsOneCounter.ContainsKey(userItem.GetBaseItem().ItemName))
            {
                itemsOneCounter.Add(userItem.GetBaseItem().ItemName, 1);
            }
            else
            {
                itemsOneCounter[userItem.GetBaseItem().ItemName]++;
            }
        }

        foreach (var userItem in this.GetTradeUser(this._twoId).OfferedItems)
        {
            if (!itemsTwoCounter.ContainsKey(userItem.GetBaseItem().ItemName))
            {
                itemsTwoCounter.Add(userItem.GetBaseItem().ItemName, 1);
            }
            else
            {
                itemsTwoCounter[userItem.GetBaseItem().ItemName]++;
            }
        }

        var logsOneString = "";
        foreach (var logs in itemsOneCounter)
        {
            logsOneString += $"{logs.Key} ({logs.Value}),";
        }

        logsOneString = logsOneString.TrimEnd(',');

        var logsTwoString = "";
        foreach (var logs in itemsTwoCounter)
        {
            logsTwoString += $"{logs.Key} ({logs.Value}),";
        }

        logsTwoString = logsTwoString.TrimEnd(',');

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        LogTradeDao.Insert(dbClient, this._oneId, this._twoId, logsOneString, logsTwoString, this._roomId);
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
            _ = room.ActiveTrades.Remove(this);
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
