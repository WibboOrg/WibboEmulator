using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Navigator.New;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Items;using Butterfly.HabboHotel.Rooms;using System;using System.Data;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class OpenGiftEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            int PresentId = Packet.PopInt();
            Item Present = Room.GetRoomItemHandler().GetItem(PresentId);
            if (Present == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            if (Present.GetBaseItem().InteractionType == InteractionType.GIFT)
            {
                DataRow Data = null;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `base_id`,`extra_data` FROM `user_presents` WHERE `item_id` = @presentId LIMIT 1");
                    dbClient.AddParameter("presentId", Present.Id);
                    Data = dbClient.GetRow();
                }

                if (Data == null)
                {
                    Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = '" + Present.Id + "'");
                        dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                    return;
                }

                if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Data["base_id"]), out ItemData BaseItem))
                {
                    Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = '" + Present.Id + "'");
                        dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                    return;
                }

                this.FinishOpenGift(Session, BaseItem, Present, Room, Data);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
            {
                ItemExtrabox.OpenExtrabox(Session, Present, Room);
            }            else if (Present.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
            {
                ItemExtrabox.OpenDeluxeBox(Session, Present, Room);
            }            else if (Present.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
            {
                ItemExtrabox.OpenLegendBox(Session, Present, Room);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
            {
                ItemExtrabox.OpenBadgeBox(Session, Present, Room);
            }        }        private void FinishOpenGift(GameClient Session, ItemData BaseItem, Item Present, Room Room, DataRow Row)
        {
            bool ItemIsInRoom = true;

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `base_item` = @BaseItem, `extra_data` = @edata WHERE `id` = @itemId LIMIT 1");
                dbClient.AddParameter("itemId", Present.Id);
                dbClient.AddParameter("BaseItem", Row["base_id"]);
                dbClient.AddParameter("edata", Row["extra_data"]);
                dbClient.RunQuery();

                dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = " + Present.Id + " LIMIT 1");
            }

            Present.BaseItem = Convert.ToInt32(Row["base_id"]);
            Present.ResetBaseItem();
            Present.ExtraData = (!string.IsNullOrEmpty(Convert.ToString(Row["extra_data"])) ? Convert.ToString(Row["extra_data"]) : "");


            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                        dbClient.AddParameter("itemId", Present.Id);
                        dbClient.RunQuery();
                    }

                    Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("itemId", Present.Id);
                    dbClient.RunQuery();
                }

                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));
        }    }}