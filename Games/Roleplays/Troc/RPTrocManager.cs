namespace WibboEmulator.Games.Roleplays.Troc;
using System.Collections.Concurrent;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;
using WibboEmulator.Games.Roleplays.Item;

public static class RPTrocManager
{
    private static int _tradeId;
    private static readonly ConcurrentDictionary<int, RPTroc> UsersTrade = new();

    public static void Confirm(int trocId, int userId)
    {
        var troc = GetTroc(trocId);
        if (troc == null)
        {
            return;
        }

        var userTroc = troc.GetUser(userId);
        if (userTroc == null || userTroc.Confirmed)
        {
            return;
        }

        if (!troc.AllAccepted)
        {
            return;
        }

        userTroc.Confirmed = true;

        if (!troc.AllConfirmed)
        {
            return;
        }

        if (!EndTrade(troc))
        {
            //SendPacket error ?
        }

        CloseTrade(troc);
    }

    public static void Accept(int trocId, int userId)
    {
        var troc = GetTroc(trocId);
        if (troc == null)
        {
            return;
        }

        var userTroc = troc.GetUser(userId);
        if (userTroc == null)
        {
            return;
        }

        if (troc.AllAccepted || troc.AllConfirmed)
        {
            return;
        }

        if (userTroc.Accepted)
        {
            userTroc.Accepted = false;
        }
        else
        {
            userTroc.Accepted = true;
        }

        SendPacketUsers(new RpTrocAccepteComposer(userId, userTroc.Accepted), troc);
    }


    private static void SendPacketUsers(IServerPacket packet, RPTroc troc)
    {
        var rpManager = RoleplayManager.GetRolePlay(troc.RPId);
        if (rpManager == null)
        {
            return;
        }

        var playerOne = rpManager.GetPlayer(troc.UserOne.UserId);
        playerOne?.SendPacket(packet);

        var playerTwo = rpManager.GetPlayer(troc.UserTwo.UserId);
        playerTwo?.SendPacket(packet);
    }

    private static void CloseTrade(RPTroc troc)
    {
        var rpManager = RoleplayManager.GetRolePlay(troc.RPId);
        if (rpManager == null)
        {
            return;
        }

        var playerOne = rpManager.GetPlayer(troc.UserOne.UserId);
        if (playerOne != null)
        {
            playerOne.TradeId = 0;
            playerOne.SendPacket(new RpTrocStopComposer());
        }

        var playerTwo = rpManager.GetPlayer(troc.UserTwo.UserId);
        if (playerTwo != null)
        {
            playerTwo.TradeId = 0;
            playerTwo.SendPacket(new RpTrocStopComposer());
        }

        _ = UsersTrade.TryRemove(troc.Id, out _);
    }

    private static bool EndTrade(RPTroc troc)
    {
        var rpManager = RoleplayManager.GetRolePlay(troc.RPId);
        if (rpManager == null)
        {
            return false;
        }

        var playerOne = rpManager.GetPlayer(troc.UserOne.UserId);
        if (playerOne == null)
        {
            return false;
        }

        var playerTwo = rpManager.GetPlayer(troc.UserTwo.UserId);
        if (playerTwo == null)
        {
            return false;
        }

        foreach (var entry in troc.UserOne.ItemIds)
        {
            var item = playerOne.GetInventoryItem(entry.Key);
            if (item == null)
            {
                return false;
            }

            if (item.Count < entry.Value)
            {
                return false;
            }
        }
        foreach (var entry in troc.UserTwo.ItemIds)
        {
            var item = playerTwo.GetInventoryItem(entry.Key);
            if (item == null)
            {
                return false;
            }

            if (item.Count < entry.Value)
            {
                return false;
            }
        }

        foreach (var entry in troc.UserOne.ItemIds)
        {
            var item = playerOne.GetInventoryItem(entry.Key);
            if (item == null)
            {
                return false;
            }

            playerOne.RemoveInventoryItem(entry.Key, entry.Value);
            playerTwo.AddInventoryItem(entry.Key, entry.Value);
        }

        foreach (var entry in troc.UserTwo.ItemIds)
        {
            playerTwo.RemoveInventoryItem(entry.Key, entry.Value);
            playerOne.AddInventoryItem(entry.Key, entry.Value);
        }

        return true;
    }

    public static void AddItem(int tradeId, int userId, int itemId)
    {
        var troc = GetTroc(tradeId);
        if (troc == null)
        {
            return;
        }

        if (troc.AllAccepted || troc.AllConfirmed)
        {
            return;
        }

        var trocUser = troc.GetUser(userId);
        if (trocUser == null || trocUser.Accepted || trocUser.Confirmed)
        {
            return;
        }

        var rpItem = RPItemManager.GetItem(itemId);
        if (rpItem == null || rpItem.Category == RPItemCategory.Quete || !rpItem.AllowStack)
        {
            return;
        }

        var rpManager = RoleplayManager.GetRolePlay(troc.RPId);
        if (rpManager == null)
        {
            return;
        }

        var player = rpManager.GetPlayer(userId);
        if (player == null)
        {
            return;
        }

        var item = player.GetInventoryItem(itemId);
        if (item == null)
        {
            return;
        }

        if (trocUser.GetCountItem(itemId) >= item.Count)
        {
            return;
        }

        trocUser.AddItemId(itemId);

        SendPacketUsers(new RpTrocUpdateItemsComposer(userId, trocUser.ItemIds), troc);
    }

    public static void RemoveItem(int tradeId, int userId, int itemId)
    {
        var troc = GetTroc(tradeId);
        if (troc == null)
        {
            return;
        }

        if (troc.AllAccepted || troc.AllConfirmed)
        {
            return;
        }

        var trocUser = troc.GetUser(userId);
        if (trocUser == null || trocUser.Accepted || trocUser.Confirmed)
        {
            return;
        }

        var rpManager = RoleplayManager.GetRolePlay(troc.RPId);
        if (rpManager == null)
        {
            return;
        }

        var player = rpManager.GetPlayer(userId);
        if (player == null)
        {
            return;
        }

        var item = player.GetInventoryItem(itemId);
        if (item == null)
        {
            return;
        }

        if (trocUser.GetCountItem(itemId) <= 0)
        {
            return;
        }

        trocUser.RemoveItemId(itemId);

        SendPacketUsers(new RpTrocUpdateItemsComposer(userId, trocUser.ItemIds), troc);
    }

    public static void AddTrade(int rpId, int userOne, int userTwo, string userNameOne, string userNameTwo)
    {
        var rpManager = RoleplayManager.GetRolePlay(rpId);
        if (rpManager == null)
        {
            return;
        }

        var playerOne = rpManager.GetPlayer(userOne);
        var playerTwo = rpManager.GetPlayer(userTwo);
        if (playerOne == null || playerTwo == null || playerOne.TradeId != 0 || playerTwo.TradeId != 0)
        {
            return;
        }

        _tradeId++;
        _ = UsersTrade.TryAdd(_tradeId, new RPTroc(_tradeId, rpId, userOne, userTwo));

        playerOne.TradeId = _tradeId;
        playerOne.SendPacket(new RpTrocStartComposer(userTwo, userNameTwo));

        playerTwo.TradeId = _tradeId;
        playerTwo.SendPacket(new RpTrocStartComposer(userOne, userNameOne));
    }

    public static void RemoveTrade(int trocId)
    {
        if (trocId == 0)
        {
            return;
        }

        _ = UsersTrade.TryGetValue(trocId, out var troc);
        if (troc == null)
        {
            return;
        }

        if (troc.AllConfirmed)
        {
            return;
        }

        CloseTrade(troc);
    }

    public static RPTroc GetTroc(int trocId)
    {
        _ = UsersTrade.TryGetValue(trocId, out var troc);
        return troc;
    }

    public static RPTrocUser GetTradeUser(int trocId, int userId)
    {
        _ = UsersTrade.TryGetValue(trocId, out var troc);
        if (troc == null)
        {
            return null;
        }

        return troc.GetUser(userId);
    }
}
