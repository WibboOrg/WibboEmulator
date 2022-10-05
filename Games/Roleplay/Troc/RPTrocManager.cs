namespace WibboEmulator.Games.Roleplay.Troc;
using System.Collections.Concurrent;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;
using WibboEmulator.Games.Roleplay.Item;

public class RPTrocManager
{
    private int _tradeId;
    private readonly ConcurrentDictionary<int, RPTroc> _usersTrade;

    public RPTrocManager() => this._usersTrade = new ConcurrentDictionary<int, RPTroc>();

    public void Confirme(int TrocId, int UserId)
    {
        var Troc = this.GetTroc(TrocId);
        if (Troc == null)
        {
            return;
        }

        var UserTroc = Troc.GetUser(UserId);
        if (UserTroc == null || UserTroc.Confirmed)
        {
            return;
        }

        if (!Troc.AllAccepted)
        {
            return;
        }

        UserTroc.Confirmed = true;

        if (!Troc.AllConfirmed)
        {
            return;
        }

        if (!EndTrade(Troc))
        {
            //SendPacket error ?
        }

        this.CloseTrade(Troc);
    }

    public void Accepte(int TrocId, int UserId)
    {
        var Troc = this.GetTroc(TrocId);
        if (Troc == null)
        {
            return;
        }

        var UserTroc = Troc.GetUser(UserId);
        if (UserTroc == null)
        {
            return;
        }

        if (Troc.AllAccepted || Troc.AllConfirmed)
        {
            return;
        }

        if (UserTroc.Accepted)
        {
            UserTroc.Accepted = false;
        }
        else
        {
            UserTroc.Accepted = true;
        }

        SendPacketUsers(new RpTrocAccepteComposer(UserId, UserTroc.Accepted), Troc);
    }


    private static void SendPacketUsers(IServerPacket packet, RPTroc Troc)
    {
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Troc.RPId);
        if (rpManager == null)
        {
            return;
        }

        var playerOne = rpManager.GetPlayer(Troc.UserOne.UserId);
        if (playerOne != null)
        {
            playerOne.SendPacket(packet);
        }

        var playerTwo = rpManager.GetPlayer(Troc.UserTwo.UserId);
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

        this._usersTrade.TryRemove(troc.Id, out troc);
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

        var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(itemId);
        if (rpItem == null || rpItem.Category == RPItemCategory.QUETE || !rpItem.AllowStack)
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
        this._usersTrade.TryAdd(this._tradeId, new RPTroc(this._tradeId, rpId, userOne, userTwo));

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

        this._usersTrade.TryGetValue(trocId, out var Troc);
        if (Troc == null)
        {
            return;
        }

        if (Troc.AllConfirmed)
        {
            return;
        }

        this.CloseTrade(Troc);
    }

    public RPTroc GetTroc(int trocId)
    {
        this._usersTrade.TryGetValue(trocId, out var troc);
        return troc;
    }

    public RPTrocUser GetTradeUser(int trocId, int userId)
    {
        this._usersTrade.TryGetValue(trocId, out var troc);
        if (troc == null)
        {
            return null;
        }

        return troc.GetUser(userId);
    }
}
