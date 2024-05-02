namespace WibboEmulator.Games.Rooms.Trading;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
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
            if (!tradeUser.RoomUser.ContainStatus("trd"))
            {
                tradeUser.RoomUser.SetStatus("trd", "");
                tradeUser.RoomUser.UpdateNeeded = true;
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
        if (tradeUser == null || item == null || !item.ItemData.AllowTrade || tradeUser.HasAccepted || this._tradeStage != 1)
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

        if (tradeUserOne.Client == null || tradeUserTwo.Client == null)
        {
            return;
        }

        if (tradeUserOne.Client.User == null || tradeUserTwo.Client.User == null)
        {
            return;
        }

        this.SendMessageToUsers(new TradingUpdateComposer(tradeUserOne.UserId, tradeUserOne.OfferedItems, tradeUserTwo.UserId, tradeUserTwo.OfferedItems));
    }

    public bool DeliverItems()
    {
        var tradeUserOne = this.GetTradeUser(this._oneId);
        var tradeUserTwo = this.GetTradeUser(this._twoId);

        if (tradeUserOne.Client == null || tradeUserTwo.Client == null)
        {
            return false;
        }

        if (tradeUserOne.Client.User == null || tradeUserTwo.Client.User == null)
        {
            return false;
        }

        var userOneItems = tradeUserOne.OfferedItems;
        var userTwoItems = tradeUserTwo.OfferedItems;

        foreach (var userItem in userOneItems)
        {
            if (tradeUserOne.Client.User.InventoryComponent.GetItem(userItem.Id) == null)
            {
                tradeUserOne.Client.SendNotification(LanguageManager.TryGetValue("trade.failed", tradeUserOne.Client.Language));
                tradeUserTwo.Client.SendNotification(LanguageManager.TryGetValue("trade.failed", tradeUserTwo.Client.Language));
                return false;
            }
        }

        foreach (var userItem in userTwoItems)
        {
            if (tradeUserTwo.Client.User.InventoryComponent.GetItem(userItem.Id) == null)
            {
                tradeUserOne.Client.SendNotification(LanguageManager.TryGetValue("trade.failed", tradeUserOne.Client.Language));
                tradeUserTwo.Client.SendNotification(LanguageManager.TryGetValue("trade.failed", tradeUserTwo.Client.Language));
                return false;
            }
        }

        using var dbClient = DatabaseManager.Connection;

        foreach (var userItem in userOneItems)
        {
            tradeUserOne.Client.User.InventoryComponent.RemoveItem(userItem.Id);
            tradeUserTwo.Client.User.InventoryComponent.AddItem(dbClient, userItem);
        }

        foreach (var userItem in userTwoItems)
        {
            tradeUserTwo.Client.User.InventoryComponent.RemoveItem(userItem.Id);
            tradeUserOne.Client.User.InventoryComponent.AddItem(dbClient, userItem);
        }

        tradeUserTwo.
        Client.SendPacket(new UnseenItemsComposer(userOneItems, UnseenItemsType.Furni));
        tradeUserOne.Client.SendPacket(new UnseenItemsComposer(userTwoItems, UnseenItemsType.Furni));

        tradeUserOne.
        Client.SendPacket(new FurniListUpdateComposer());
        tradeUserTwo.Client.SendPacket(new FurniListUpdateComposer());

        return true;
    }

    private void SaveLogs()
    {
        var itemsOneCounter = new Dictionary<string, int>();
        var itemsTwoCounter = new Dictionary<string, int>();

        foreach (var userItem in this.GetTradeUser(this._oneId).OfferedItems)
        {
            if (!itemsOneCounter.ContainsKey(userItem.ItemData.ItemName))
            {
                itemsOneCounter.Add(userItem.ItemData.ItemName, 1);
            }
            else
            {
                itemsOneCounter[userItem.ItemData.ItemName]++;
            }
        }

        foreach (var userItem in this.GetTradeUser(this._twoId).OfferedItems)
        {
            if (!itemsTwoCounter.ContainsKey(userItem.ItemData.ItemName))
            {
                itemsTwoCounter.Add(userItem.ItemData.ItemName, 1);
            }
            else
            {
                itemsTwoCounter[userItem.ItemData.ItemName]++;
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

        using var dbClient = DatabaseManager.Connection;
        LogTradeDao.Insert(dbClient, this._oneId, this._twoId, logsOneString, logsTwoString, this._roomId);
    }

    public void CloseTradeClean()
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser != null && tradeUser.RoomUser != null)
            {
                tradeUser.RoomUser.RemoveStatus("trd");
                tradeUser.RoomUser.UpdateNeeded = true;
            }
        }

        this.SendMessageToUsers(new TradingFinishComposer());

        if (RoomManager.TryGetRoom(this._roomId, out var room))
        {
            _ = room.ActiveTrades.Remove(this);
        }
    }

    public void CloseTrade(int userId)
    {
        foreach (var tradeUser in this._users)
        {
            if (tradeUser != null && tradeUser.RoomUser != null)
            {
                tradeUser.RoomUser.RemoveStatus("trd");
                tradeUser.RoomUser.UpdateNeeded = true;
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
            if (tradeUser != null && tradeUser.Client != null)
            {
                tradeUser.Client.SendPacket(message);
            }
        }
    }
}
