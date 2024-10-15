namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class PlaceObjectEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session))
        {
            Session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            Session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", Session.Language));
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

        var userItem = Session.User.InventoryComponent.GetItem(itemId);
        if (userItem == null)
        {
            return;
        }

        if (userItem.Data.IsRare && !room.CheckRights(Session, true))
        {
            Session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_trade_stuff}"));
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

            if (Session.User.ForceRot > -1)
            {
                rotation = Session.User.ForceRot;
            }

            var item = new Item(userItem.Id, room.Id, userItem.BaseItemId, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, x, y, 0.0, rotation, "", room);
            if (room.RoomItemHandling.SetFloorItem(Session, item, x, y, rotation, true, false, true))
            {
                using (var dbClient = DatabaseManager.Connection)
                {
                    ItemDao.UpdateRoomIdAndUserId(dbClient, itemId, room.Id, room.RoomData.OwnerId);
                }

                Session.User.InventoryComponent.RemoveItem(itemId);

                if (WiredUtillity.TypeIsWired(userItem.ItemData.InteractionType))
                {
                    WiredRegister.HandleRegister(room, item);
                }

                if (Session.User.ForceUse > -1)
                {
                    item.Interactor.OnTrigger(Session, item, 0, true, false);
                }

                if (Session.User.ForceOpenGift)
                {
                    if (item.ItemData.InteractionType == InteractionType.EXTRA_BOX)
                    {
                        ItemLootBox.OpenExtrabox(Session, item, room);
                    }
                    else if (item.ItemData.InteractionType == InteractionType.DELUXE_BOX)
                    {
                        ItemLootBox.OpenDeluxeBox(Session, item, room);
                    }
                    else if (item.ItemData.InteractionType == InteractionType.LOOTBOX_2022)
                    {
                        ItemLootBox.OpenLootBox2022(Session, item, room);
                    }
                    else if (item.ItemData.InteractionType == InteractionType.LEGEND_BOX)
                    {
                        ItemLootBox.OpenLegendBox(Session, item, room);
                    }
                    else if (item.ItemData.InteractionType == InteractionType.BADGE_BOX)
                    {
                        ItemLootBox.OpenBadgeBox(Session, item, room);
                    }
                    else if (item.ItemData.InteractionType == InteractionType.GIFT_BANNER)
                    {
                        item.Interactor.OnTrigger(Session, item, 0, true, false);
                    }
                    else if (item.ItemData.InteractionType is InteractionType.CASE_MIEL or InteractionType.CASE_ATHENA or InteractionType.BAG_SAKURA or InteractionType.BAG_ATLANTA or InteractionType.BAG_KYOTO)
                    {
                        ItemLootBox.OpenCaseOrBag(Session, item, room);
                    }
                }

                QuestManager.ProgressUserQuest(Session, QuestType.FurniPlace, 0);
            }
            else
            {
                Session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
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
                var roomItem = new Item(userItem.Id, room.Id, userItem.BaseItemId, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, wallPos, room);
                if (room.RoomItemHandling.SetWallItem(Session, roomItem))
                {
                    using (var dbClient = DatabaseManager.Connection)
                    {
                        ItemDao.UpdateRoomIdAndUserId(dbClient, itemId, room.Id, room.RoomData.OwnerId);
                    }

                    Session.User.InventoryComponent.RemoveItem(itemId);
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

        position = ItemWallUtility.WallPositionCheck(wallPos);

        return position != "";
    }
}
