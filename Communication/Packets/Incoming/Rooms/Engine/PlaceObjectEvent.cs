using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Wired;
using System;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class PlaceObjectEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (Room == null || !Room.CheckRights(Session))
            {
                Session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
                return;
            }

            if (Room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            string RawData = Packet.PopString();
            string[] Data = RawData.Split(new char[1] { ' ' });

            if (!int.TryParse(Data[0], out int ItemId))
            {
                return;
            }

            if (ItemId <= 0)
            {
                return;
            }

            Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (userItem == null)
            {
                return;
            }

            if (userItem.GetBaseItem().InteractionType == InteractionType.BADGE_TROC)
            {
                if (Session.GetHabbo().GetBadgeComponent().HasBadge(userItem.ExtraData))
                {
                    Session.SendNotification("Vous posséder déjà ce badge !");
                    return;
                }

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.Delete(dbClient, ItemId);
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);

                Session.GetHabbo().GetBadgeComponent().GiveBadge(userItem.ExtraData, true);
                Session.SendPacket(new ReceiveBadgeComposer(userItem.ExtraData));

                Session.SendNotification("Vous avez reçu le badge: " + userItem.ExtraData + " !");
                return;
            }

            if (!userItem.IsWallItem)
            {
                if (Data.Length < 4)
                {
                    return;
                }

                if (!int.TryParse(Data[1], out int X))
                {
                    return;
                }

                if (!int.TryParse(Data[2], out int Y))
                {
                    return;
                }

                if (!int.TryParse(Data[3], out int Rotation))
                {
                    return;
                }

                if (Session.GetHabbo().forceRot > -1)
                {
                    Rotation = Session.GetHabbo().forceRot;
                }

                Item roomItem = new Item(userItem.Id, Room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, X, Y, 0.0, Rotation, "", Room);
                if (Room.GetRoomItemHandler().SetFloorItem(Session, roomItem, X, Y, Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateRoomIdAndUserId(dbClient, ItemId, Room.Id, Room.RoomData.OwnerId);
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);

                    if (WiredUtillity.TypeIsWired(userItem.GetBaseItem().InteractionType))
                    {
                        WiredSaver.HandleDefaultSave(userItem.Id, Room, roomItem);
                    }

                    if (Session.GetHabbo().forceUse > -1)
                    {
                        roomItem.Interactor.OnTrigger(Session, roomItem, 0, true);
                    }

                    if (Session.GetHabbo().forceOpenGift)
                    {
                        if (roomItem.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
                        {
                            ItemExtrabox.OpenExtrabox(Session, roomItem, Room);
                        }
                        else if (roomItem.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
                        {
                            ItemExtrabox.OpenDeluxeBox(Session, roomItem, Room);
                        }
                        else if (roomItem.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
                        {
                            ItemExtrabox.OpenLegendBox(Session, roomItem, Room);
                        }
                        else if (roomItem.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
                        {
                            ItemExtrabox.OpenBadgeBox(Session, roomItem, Room);
                        }
                    }

                    ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PLACE, 0);
                }
                else
                {
                    Session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
                    return;
                }
            }


            else if (userItem.IsWallItem)
            {
                string[] CorrectedData = new string[Data.Length - 1];

                for (int i = 1; i < Data.Length; i++)
                {
                    CorrectedData[i - 1] = Data[i];
                }

                string WallPos = string.Empty;
                if (TrySetWallItem(CorrectedData, out WallPos))
                {
                    Item roomItem = new Item(userItem.Id, Room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, WallPos, Room);
                    if (Room.GetRoomItemHandler().SetWallItem(Session, roomItem))
                    {
                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            ItemDao.UpdateRoomIdAndUserId(dbClient, ItemId, Room.Id, Room.RoomData.OwnerId);
                        }

                        Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                    }
                }
                else
                {
                    Session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
                    return;
                }
            }
        }


        private static bool TrySetWallItem(string[] data, out string position)
        {
            if (data.Length < 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || (data[2] != "r" && data[2] != "l"))
            {
                position = null;
                return false;
            }

            string wBit = data[0].Substring(3, data[0].Length - 3);
            string lBit = data[1].Substring(2, data[1].Length - 2);

            if (!wBit.Contains(",") || !lBit.Contains(","))
            {
                position = null;
                return false;
            }


            int.TryParse(wBit.Split(',')[0], out int w1);
            int.TryParse(wBit.Split(',')[1], out int w2);
            int.TryParse(lBit.Split(',')[0], out int l1);
            int.TryParse(lBit.Split(',')[1], out int l2);

            string WallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

            position = WallPositionCheck(WallPos);

            return (position != null);
        }

        private static string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                {
                    return null;
                }

                if (wallPosition.Contains(Convert.ToChar(9)))
                {
                    return null;
                }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                {
                    return null;
                }

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                //if (widthX < -1000 || widthY < -1 || widthX > 700 || widthY > 700)
                //return null;

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                //if (lengthX < -1 || lengthY < -1000 || lengthX > 700 || lengthY > 700)
                //return null;

                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return null;
            }
        }
    }
}
