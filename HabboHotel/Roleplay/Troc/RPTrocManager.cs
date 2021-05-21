using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.WebSocket.Troc;
using Butterfly.HabboHotel.Roleplay.Item;
using Butterfly.HabboHotel.Roleplay.Player;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Roleplay.Troc
{
    public class RPTrocManager
    {
        private int _tradeId;
        private readonly ConcurrentDictionary<int, RPTroc> _usersTrade;

        public RPTrocManager()
        {
            this._usersTrade = new ConcurrentDictionary<int, RPTroc>();
        }

        public void Confirme(int TrocId, int UserId)
        {
            RPTroc Troc = this.GetTroc(TrocId);
            if (Troc == null)
            {
                return;
            }

            RPTrocUser UserTroc = Troc.GetUser(UserId);
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

            if (!this.EndTrade(Troc))
            {
                //SendPacket error ?
            }

            this.CloseTrade(Troc);
        }

        public void Accepte(int TrocId, int UserId)
        {
            RPTroc Troc = this.GetTroc(TrocId);
            if (Troc == null)
            {
                return;
            }

            RPTrocUser UserTroc = Troc.GetUser(UserId);
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

            this.SendPacketUsers(new RpTrocAccepteComposer(UserId, UserTroc.Accepted), Troc);
        }


        private void SendPacketUsers(IServerPacket packet, RPTroc Troc)
        {
            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Troc.RPId);
            if (RPManager == null)
            {
                return;
            }

            RolePlayer PlayerOne = RPManager.GetPlayer(Troc.UserOne.UserId);
            if (PlayerOne != null)
            {
                PlayerOne.SendWebPacket(packet);
            }

            RolePlayer PlayerTwo = RPManager.GetPlayer(Troc.UserTwo.UserId);
            if (PlayerTwo != null)
            {
                PlayerTwo.SendWebPacket(packet);
            }
        }

        private void CloseTrade(RPTroc Troc)
        {
            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Troc.RPId);
            if (RPManager == null)
            {
                return;
            }

            RolePlayer PlayerOne = RPManager.GetPlayer(Troc.UserOne.UserId);
            if (PlayerOne != null)
            {
                PlayerOne.TradeId = 0;
                PlayerOne.SendWebPacket(new RpTrocStopComposer());
            }

            RolePlayer PlayerTwo = RPManager.GetPlayer(Troc.UserTwo.UserId);
            if (PlayerTwo != null)
            {
                PlayerTwo.TradeId = 0;
                PlayerTwo.SendWebPacket(new RpTrocStopComposer());
            }

            this._usersTrade.TryRemove(Troc.Id, out Troc);
        }

        private bool EndTrade(RPTroc Troc)
        {
            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Troc.RPId);
            if (RPManager == null)
            {
                return false;
            }

            RolePlayer PlayerOne = RPManager.GetPlayer(Troc.UserOne.UserId);
            if (PlayerOne == null)
            {
                return false;
            }

            RolePlayer PlayerTwo = RPManager.GetPlayer(Troc.UserTwo.UserId);
            if (PlayerTwo == null)
            {
                return false;
            }

            foreach (KeyValuePair<int, int> entry in Troc.UserOne.ItemIds)
            {
                RolePlayInventoryItem Item = PlayerOne.GetInventoryItem(entry.Key);
                if (Item == null)
                {
                    return false;
                }

                if (Item.Count < entry.Value)
                {
                    return false;
                }
            }
            foreach (KeyValuePair<int, int> entry in Troc.UserTwo.ItemIds)
            {
                RolePlayInventoryItem Item = PlayerTwo.GetInventoryItem(entry.Key);
                if (Item == null)
                {
                    return false;
                }

                if (Item.Count < entry.Value)
                {
                    return false;
                }
            }

            foreach (KeyValuePair<int, int> entry in Troc.UserOne.ItemIds)
            {
                RolePlayInventoryItem Item = PlayerOne.GetInventoryItem(entry.Key);
                if (Item == null)
                {
                    return false;
                }

                PlayerOne.RemoveInventoryItem(entry.Key, entry.Value);
                PlayerTwo.AddInventoryItem(entry.Key, entry.Value);
            }

            foreach (KeyValuePair<int, int> entry in Troc.UserTwo.ItemIds)
            {
                PlayerTwo.RemoveInventoryItem(entry.Key, entry.Value);
                PlayerOne.AddInventoryItem(entry.Key, entry.Value);
            }

            return true;
        }

        public void AddItem(int TradeId, int UserId, int ItemId)
        {
            RPTroc Troc = this.GetTroc(TradeId);
            if (Troc == null)
            {
                return;
            }

            if (Troc.AllAccepted || Troc.AllConfirmed)
            {
                return;
            }

            RPTrocUser TrocUser = Troc.GetUser(UserId);
            if (TrocUser == null || TrocUser.Accepted || TrocUser.Confirmed)
            {
                return;
            }

            RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
            if (RpItem == null || RpItem.Category == RPItemCategory.QUETE || !RpItem.AllowStack)
            {
                return;
            }

            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Troc.RPId);
            if (RPManager == null)
            {
                return;
            }

            RolePlayer Player = RPManager.GetPlayer(UserId);
            if (Player == null)
            {
                return;
            }

            RolePlayInventoryItem Item = Player.GetInventoryItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (TrocUser.GetCountItem(ItemId) >= Item.Count)
            {
                return;
            }

            TrocUser.AddItemId(ItemId);

            this.SendPacketUsers(new RpTrocUpdateItemsComposer(UserId, TrocUser.ItemIds), Troc);
        }

        public void RemoveItem(int TradeId, int UserId, int ItemId)
        {
            RPTroc Troc = this.GetTroc(TradeId);
            if (Troc == null)
            {
                return;
            }

            if (Troc.AllAccepted || Troc.AllConfirmed)
            {
                return;
            }

            RPTrocUser TrocUser = Troc.GetUser(UserId);
            if (TrocUser == null || TrocUser.Accepted || TrocUser.Confirmed)
            {
                return;
            }

            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Troc.RPId);
            if (RPManager == null)
            {
                return;
            }

            RolePlayer Player = RPManager.GetPlayer(UserId);
            if (Player == null)
            {
                return;
            }

            RolePlayInventoryItem Item = Player.GetInventoryItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (TrocUser.GetCountItem(ItemId) <= 0)
            {
                return;
            }

            TrocUser.RemoveItemId(ItemId);

            this.SendPacketUsers(new RpTrocUpdateItemsComposer(UserId, TrocUser.ItemIds), Troc);
        }

        public void AddTrade(int RPId, int UserOne, int UserTwo, string UserNameOne, string UserNameTwo)
        {
            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(RPId);
            if (RPManager == null)
            {
                return;
            }

            RolePlayer PlayerOne = RPManager.GetPlayer(UserOne);
            RolePlayer PlayerTwo = RPManager.GetPlayer(UserTwo);
            if (PlayerOne == null || PlayerTwo == null || PlayerOne.TradeId != 0 || PlayerTwo.TradeId != 0)
            {
                return;
            }

            this._tradeId++;
            this._usersTrade.TryAdd(this._tradeId, new RPTroc(this._tradeId, RPId, UserOne, UserTwo));

            PlayerOne.TradeId = this._tradeId;
            PlayerOne.SendWebPacket(new RpTrocStartComposer(UserTwo, UserNameTwo));

            PlayerTwo.TradeId = this._tradeId;
            PlayerTwo.SendWebPacket(new RpTrocStartComposer(UserOne, UserNameOne));
        }

        public void RemoveTrade(int TrocId)
        {
            if (TrocId == 0)
            {
                return;
            }

            this._usersTrade.TryGetValue(TrocId, out RPTroc Troc);
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

        public RPTroc GetTroc(int TrocId)
        {
            this._usersTrade.TryGetValue(TrocId, out RPTroc Troc);
            return Troc;
        }

        public RPTrocUser GetTradeUser(int TrocId, int UserId)
        {
            this._usersTrade.TryGetValue(TrocId, out RPTroc Troc);
            if (Troc == null)
            {
                return null;
            }

            return Troc.GetUser(UserId);
        }
    }
}
