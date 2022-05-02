using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using System;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms
{
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
                foreach(TradeUser tradeUser in this._users)
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

            foreach (TradeUser tradeUser in this._users)
            {
                if (!tradeUser.GetRoomUser().Statusses.ContainsKey("/trd"))
                {
                    tradeUser.GetRoomUser().SetStatus("/trd", "");
                    tradeUser.GetRoomUser().UpdateNeeded = true;
                }
            }
        }

        public bool ContainsUser(int Id)
        {
            foreach (TradeUser tradeUser in this._users)
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
            foreach (TradeUser tradeUser in this._users)
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
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || Item == null || (!Item.GetBaseItem().AllowTrade || tradeUser.HasAccepted) || this._tradeStage != 1)
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
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || Item == null || (tradeUser.HasAccepted || this._tradeStage != 1))
            {
                return;
            }

            this.ClearAccepted();
            tradeUser.OfferedItems.Remove(Item);
            this.UpdateTradeWindow();
        }

        public void Accept(int UserId)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
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
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || this._tradeStage != 1 || this.AllUsersAccepted)
            {
                return;
            }

            tradeUser.HasAccepted = false;
            this.SendMessageToUsers(new TradingAcceptComposer(UserId, 0));
        }

        public void CompleteTrade(int UserId)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
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
                ExceptionLogger.LogThreadException((ex).ToString(), "Trade task");
            }
        }

        public void ClearAccepted()
        {
            foreach (TradeUser tradeUser in this._users)
            {
                tradeUser.HasAccepted = false;
            }
        }

        public void UpdateTradeWindow()
        {
            TradeUser tradeUserOne = this.GetTradeUser(this._oneId);
            TradeUser tradeUserTwo = this.GetTradeUser(this._twoId);

            if (tradeUserOne.GetClient() == null || tradeUserTwo.GetClient() == null)
                return;

            if (tradeUserOne.GetClient().GetUser() == null || tradeUserTwo.GetClient().GetUser() == null)
                return;

            this.SendMessageToUsers(new TradingUpdateComposer(tradeUserOne.UserId, tradeUserOne.OfferedItems, tradeUserTwo.UserId, tradeUserTwo.OfferedItems));
        }

        public bool DeliverItems()
        {
            TradeUser tradeUserOne = this.GetTradeUser(this._oneId);
            TradeUser tradeUserTwo = this.GetTradeUser(this._twoId);

            if (tradeUserOne.GetClient() == null || tradeUserTwo.GetClient() == null)
                return false;

            if (tradeUserOne.GetClient().GetUser() == null || tradeUserTwo.GetClient().GetUser() == null)
                return false;

            List<Item> userOneItems = tradeUserOne.OfferedItems;
            List<Item> userTwoItems = tradeUserTwo.OfferedItems;

            foreach (Item userItem in userOneItems)
            {
                if (tradeUserOne.GetClient().GetUser().GetInventoryComponent().GetItem(userItem.Id) == null)
                {
                    tradeUserOne.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserOne.GetClient().Langue));
                    tradeUserTwo.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserTwo.GetClient().Langue));
                    return false;
                }
            }

            foreach (Item userItem in userTwoItems)
            {
                if (tradeUserTwo.GetClient().GetUser().GetInventoryComponent().GetItem(userItem.Id) == null)
                {
                    tradeUserOne.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserOne.GetClient().Langue));
                    tradeUserTwo.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("trade.failed", tradeUserTwo.GetClient().Langue));
                    return false;
                }
            }

            foreach (Item userItem in userOneItems)
            {
                tradeUserOne.GetClient().GetUser().GetInventoryComponent().RemoveItem(userItem.Id);
                tradeUserTwo.GetClient().GetUser().GetInventoryComponent().AddItem(userItem);
            }

            foreach (Item userItem in userTwoItems)
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
            Dictionary<string, int> ItemsOneCounter = new Dictionary<string, int>();
            Dictionary<string, int> ItemsTwoCounter = new Dictionary<string, int>();

            foreach (Item userItem in this.GetTradeUser(this._oneId).OfferedItems)
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

            foreach (Item userItem in this.GetTradeUser(this._twoId).OfferedItems)
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

            string LogsOneString = "";
            foreach (KeyValuePair<string, int> Logs in ItemsOneCounter)
            {
                LogsOneString += $"{Logs.Key} ({Logs.Value}),";
            }

            LogsOneString = LogsOneString.TrimEnd(',');

            string LogsTwoString = "";
            foreach (KeyValuePair<string, int> Logs in ItemsTwoCounter)
            {
                LogsTwoString += $"{Logs.Key} ({Logs.Value}),";
            }

            LogsTwoString = LogsTwoString.TrimEnd(',');

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                LogTradeDao.Insert(dbClient, this._oneId, this._twoId, LogsOneString, LogsTwoString, this._roomId);
            }
        }

        public void CloseTradeClean()
        {
            foreach(TradeUser tradeUser in this._users)
            {
                if (tradeUser != null && tradeUser.GetRoomUser() != null)
                {
                    tradeUser.GetRoomUser().RemoveStatus("/trd");
                    tradeUser.GetRoomUser().UpdateNeeded = true;
                }
            }

            this.SendMessageToUsers(new TradingFinishComposer());
            this.GetRoom().ActiveTrades.Remove(this);
        }

        public void CloseTrade(int UserId)
        {
            foreach (TradeUser tradeUser in this._users)
            {
                if (tradeUser != null && tradeUser.GetRoomUser() != null)
                {
                    tradeUser.GetRoomUser().RemoveStatus("/trd");
                    tradeUser.GetRoomUser().UpdateNeeded = true;
                }
            }

            this.SendMessageToUsers(new TradingClosedComposer(UserId));
        }

        public void SendMessageToUsers(ServerPacket Message)
        {
            if (this._users == null)
            {
                return;
            }

            foreach (TradeUser tradeUser in this._users)
            {
                if (tradeUser != null && tradeUser.GetClient() != null)
                {
                    tradeUser.GetClient().SendPacket(Message);
                }
            }
        }

        private Room GetRoom()
        {
            return ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this._roomId);
        }
    }
}