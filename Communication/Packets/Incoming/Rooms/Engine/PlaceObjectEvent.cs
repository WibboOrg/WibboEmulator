namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Quests;

internal class PlaceObjectEvent : IPacketEvent
{
    public double Delay => 200;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        var rawData = packet.PopString();
        var data = rawData.Split(' ');

        if (!int.TryParse(data[0], out var itemId))
        {
            return;
        }

        if (itemId <= 0)
        {
            return;
        }

        var userItem = session.GetUser().GetInventoryComponent().GetItem(itemId);
        if (userItem == null)
        {
            return;
        }

        if (userItem.Data.IsRare && !room.CheckRights(session, true))
        {
            session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_trade_stuff}"));
            return;
        }

        if (userItem.GetBaseItem().InteractionType == InteractionType.BADGE_TROC)
        {
            if (session.GetUser().GetBadgeComponent().HasBadge(userItem.ExtraData))
            {
                session.SendNotification("Vous posséder déjà ce badge !");
                return;
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.Delete(dbClient, itemId);
            }

            session.GetUser().GetInventoryComponent().RemoveItem(itemId);

            session.GetUser().GetBadgeComponent().GiveBadge(userItem.ExtraData, true);
            session.SendPacket(new ReceiveBadgeComposer(userItem.ExtraData));

            session.SendNotification("Vous avez reçu le badge: " + userItem.ExtraData + " !");
            return;
        }

        if (!userItem.IsWallItem)
        {
            if (data.Length < 4)
            {
                return;
            }

            if (!int.TryParse(data[1], out var x))
            {
                return;
            }

            if (!int.TryParse(data[2], out var y))
            {
                return;
            }

            if (!int.TryParse(data[3], out var rotation))
            {
                return;
            }

            if (session.GetUser().ForceRot > -1)
            {
                rotation = session.GetUser().ForceRot;
            }

            var item = new Item(userItem.Id, room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, x, y, 0.0, rotation, "", room);
            if (room.RoomItemHandling.SetFloorItem(session, item, x, y, rotation, true, false, true))
            {
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateRoomIdAndUserId(dbClient, itemId, room.Id, room.RoomData.OwnerId);
                }

                session.GetUser().GetInventoryComponent().RemoveItem(itemId);

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
                        ItemLootBox.OpenExtrabox(session, item, room);
                    }
                    else if (item.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
                    {
                        ItemLootBox.OpenDeluxeBox(session, item, room);
                    }
                    else if (item.GetBaseItem().InteractionType == InteractionType.LOOTBOX2022)
                    {
                        ItemLootBox.OpenLootBox2022(session, item, room);
                    }
                    else if (item.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
                    {
                        ItemLootBox.OpenLegendBox(session, item, room);
                    }
                    else if (item.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
                    {
                        ItemLootBox.OpenBadgeBox(session, item, room);
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
            var correctedData = new string[data.Length - 1];

            for (var i = 1; i < data.Length; i++)
            {
                correctedData[i - 1] = data[i];
            }

            if (TrySetWallItem(correctedData, out var wallPos))
            {
                var roomItem = new Item(userItem.Id, room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, wallPos, room);
                if (room.RoomItemHandling.SetWallItem(session, roomItem))
                {
                    using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateRoomIdAndUserId(dbClient, itemId, room.Id, room.RoomData.OwnerId);
                    }

                    session.GetUser().GetInventoryComponent().RemoveItem(itemId);
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
            position = "";
            return false;
        }

        var wBit = data[0][3..];
        var lBit = data[1][2..];

        if (!wBit.Contains(',') || !lBit.Contains(','))
        {
            position = "";
            return false;
        }


        _ = int.TryParse(wBit.Split(',')[0], out var w1);
        _ = int.TryParse(wBit.Split(',')[1], out var w2);
        _ = int.TryParse(lBit.Split(',')[0], out var l1);
        _ = int.TryParse(lBit.Split(',')[1], out var l2);

        var wallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

        position = WallPositionCheck(wallPos);

        return position != "";
    }

    private static string WallPositionCheck(string wallPosition)
    {
        //:w=3,2 l=9,63 l
        try
        {
            if (wallPosition.Contains(Convert.ToChar(13)))
            {
                return "";
            }

            if (wallPosition.Contains(Convert.ToChar(9)))
            {
                return "";
            }

            var posD = wallPosition.Split(' ');
            if (posD[2] is not "l" and not "r")
            {
                return "";
            }

            var widD = posD[0][3..].Split(',');
            var widthX = int.Parse(widD[0]);
            var widthY = int.Parse(widD[1]);
            //if (widthX < -1000 || widthY < -1 || widthX > 700 || widthY > 700)
            //return null;

            var lenD = posD[1][2..].Split(',');
            var lengthX = int.Parse(lenD[0]);
            var lengthY = int.Parse(lenD[1]);
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
