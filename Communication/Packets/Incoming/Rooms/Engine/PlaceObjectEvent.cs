using Wibbo.Communication.Packets.Outgoing.Rooms.Notifications;
using Wibbo.Communication.Packets.Outgoing.Users;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Items.Wired;
using Wibbo.Game.Quests;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class PlaceObjectEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(Client session, ClientPacket Packet)
        {
            if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(session))
            {
                session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
                return;
            }

            if (room.RoomData.SellPrice > 0)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
                return;
            }

            string RawData = Packet.PopString();
            string[] Data = RawData.Split(' ');

            if (!int.TryParse(Data[0], out int ItemId))
            {
                return;
            }

            if (ItemId <= 0)
            {
                return;
            }

            Item userItem = session.GetUser().GetInventoryComponent().GetItem(ItemId);
            if (userItem == null)
            {
                return;
            }

            if (userItem.GetBaseItem().InteractionType == InteractionType.BADGE_TROC)
            {
                if (session.GetUser().GetBadgeComponent().HasBadge(userItem.ExtraData))
                {
                    session.SendNotification("Vous posséder déjà ce badge !");
                    return;
                }

                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.Delete(dbClient, ItemId);
                }

                session.GetUser().GetInventoryComponent().RemoveItem(ItemId);

                session.GetUser().GetBadgeComponent().GiveBadge(userItem.ExtraData, true);
                session.SendPacket(new ReceiveBadgeComposer(userItem.ExtraData));

                session.SendNotification("Vous avez reçu le badge: " + userItem.ExtraData + " !");
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

                if (!int.TryParse(Data[3], out int rotation))
                {
                    return;
                }

                if (session.GetUser().ForceRot > -1)
                {
                    rotation = session.GetUser().ForceRot;
                }

                Item item = new Item(userItem.Id, room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, X, Y, 0.0, rotation, "", room);
                if (room.GetRoomItemHandler().SetFloorItem(session, item, X, Y, rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateRoomIdAndUserId(dbClient, ItemId, room.Id, room.RoomData.OwnerId);
                    }

                    session.GetUser().GetInventoryComponent().RemoveItem(ItemId);

                    if (WiredUtillity.TypeIsWired(userItem.GetBaseItem().InteractionType))
                    {
                        WiredRegister.HandleRegister(room, item);
                    }

                    if (session.GetUser().ForceUse > -1)
                    {
                        item.Interactor.OnTrigger(session, item, 0, true, false);
                    }

                    if (session.GetUser().ForceOpenGift)
                    {
                        if (item.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
                        {
                            ItemLotBox.OpenExtrabox(session, item, room);
                        }
                        else if (item.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
                        {
                            ItemLotBox.OpenDeluxeBox(session, item, room);
                        }
                        else if (item.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
                        {
                            ItemLotBox.OpenLegendBox(session, item, room);
                        }
                        else if (item.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
                        {
                            ItemLotBox.OpenBadgeBox(session, item, room);
                        }
                    }

                    WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FURNI_PLACE, 0);
                }
                else
                {
                    session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
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

                if (TrySetWallItem(CorrectedData, out string wallPos))
                {
                    Item roomItem = new Item(userItem.Id, room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, wallPos, room);
                    if (room.GetRoomItemHandler().SetWallItem(session, roomItem))
                    {
                        using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            ItemDao.UpdateRoomIdAndUserId(dbClient, ItemId, room.Id, room.RoomData.OwnerId);
                        }

                        session.GetUser().GetInventoryComponent().RemoveItem(ItemId);
                    }
                }
                else
                {
                    session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
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
