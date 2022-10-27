namespace WibboEmulator.Games.Roleplays.Troc;
using System.Collections.Concurrent;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;
using WibboEmulator.Games.Roleplays.Item;

public class RPTrocManager
{
    private int _tradeId;
    private readonly ConcurrentDictionary<int, RPTroc> _usersTrade;

    public RPTrocManager() => this._usersTrade = new ConcurrentDictionary<int, RPTroc>();

    public void Confirme(int trocId, int userId)
    {
        var troc = this.GetTroc(trocId);
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

        this.CloseTrade(troc);
    }

    public void Accepte(int trocId, int userId)
    {
        var troc = this.GetTroc(trocId);
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
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(troc.RPId);
        if (rpManager == null)
        {
            return;
        }

        var playerOne = rpManager.GetPlayer(troc.UserOne.UserId);
        if (playerOne != null)
        {
            playerOne.SendPacket(packet);
        }

        var playerTwo = rpManager.GetPlayer(troc.UserTwo.UserId);
        if (playerTwo != null)
        {
            playerTwo.SendPacket(packet);
        }
    }

    private void CloseTrade(RPTroc troc)
    {
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(troc.RPId);
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

        _ = this._usersTrade.TryRemove(troc.Id, out _);
    }

    private static bool EndTrade(RPTroc troc)
    {
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(troc.RPId);
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

    public void AddItem(int tradeId, int userId, int itemId)
    {
        var troc = this.GetTroc(tradeId);
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

        var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().ItemManager.GetItem(itemId);
        if (rpItem == null || rpItem.Category == RPItemCategory.Quete || !rpItem.AllowStack)
        {
            return;
        }

        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(troc.RPId);
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

    public void RemoveItem(int tradeId, int userId, int itemId)
    {
        var troc = this.GetTroc(tradeId);
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

        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(troc.RPId);
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

    public void AddTrade(int rpId, int userOne, int userTwo, string userNameOne, string userNameTwo)
    {
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(rpId);
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

        this._tradeId++;
        _ = this._usersTrade.TryAdd(this._tradeId, new RPTroc(this._tradeId, rpId, userOne, userTwo));

        playerOne.TradeId = this._tradeId;
        playerOne.SendPacket(new RpTrocStartComposer(userTwo, userNameTwo));

        playerTwo.TradeId = this._tradeId;
        playerTwo.SendPacket(new RpTrocStartComposer(userOne, userNameOne));
    }

    public void RemoveTrade(int trocId)
    {
        if (trocId == 0)
        {
            return;
        }

        _ = this._usersTrade.TryGetValue(trocId, out var troc);
        if (troc == null)
        {
            return;
        }

        if (troc.AllConfirmed)
        {
            return;
        }

        this.CloseTrade(troc);
    }

    public RPTroc GetTroc(int trocId)
    {
        _ = this._usersTrade.TryGetValue(trocId, out var troc);
        return troc;
    }

    public RPTrocUser GetTradeUser(int trocId, int userId)
    {
        _ = this._usersTrade.TryGetValue(trocId, out var troc);
        if (troc == null)
        {
            return null;
        }

        return troc.GetUser(userId);
    }
}
