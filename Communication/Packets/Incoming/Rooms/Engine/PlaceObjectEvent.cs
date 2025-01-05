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
using WibboEmulator.Games.Rooms.Moodlight;

internal sealed class PlaceObjectEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
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
            session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", session.Language));
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

        var userItem = session.User.InventoryComponent.GetItem(itemId);
        if (userItem == null)
        {
            return;
        }

        if (userItem.Data.IsRare && !room.CheckRights(session, true))
        {
            session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_trade_stuff}"));
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

            if (session.User.ForceRot > -1)
            {
                rotation = session.User.ForceRot;
            }

            var roomItem = new Item(userItem.Id, room.Id, userItem.BaseItemId, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, x, y, 0.0, rotation, "", room);
            if (room.RoomItemHandling.SetFloorItem(session, roomItem, x, y, rotation, true, false, true))
            {
                using (var dbClient = DatabaseManager.Connection)
                {
                    ItemDao.UpdateRoomIdAndUserId(dbClient, itemId, room.Id, room.RoomData.OwnerId);

                    if (roomItem.Data.InteractionType is InteractionType.TELEPORT or InteractionType.TELEPORT_ARROW)
                    {
                        var teleTwoId = ItemTeleportDao.GetOne(dbClient, roomItem.Id);
                        roomItem.SetTeleLinkId(teleTwoId);
                    }
                }

                session.User.InventoryComponent.RemoveItem(itemId);

                if (WiredUtillity.TypeIsWired(userItem.ItemData.InteractionType))
                {
                    WiredRegister.HandleRegister(room, roomItem);
                }

                if (session.User.ForceUse > -1)
                {
                    roomItem.Interactor.OnTrigger(session, roomItem, 0, true, false);
                }

                if (session.User.ForceOpenGift)
                {
                    if (roomItem.ItemData.InteractionType == InteractionType.EXTRA_BOX)
                    {
                        ItemLootBox.OpenExtrabox(session, roomItem, room);
                    }
                    else if (roomItem.ItemData.InteractionType == InteractionType.DELUXE_BOX)
                    {
                        ItemLootBox.OpenDeluxeBox(session, roomItem, room);
                    }
                    else if (roomItem.ItemData.InteractionType == InteractionType.LOOTBOX_2022)
                    {
                        ItemLootBox.OpenLootBox2022(session, roomItem, room);
                    }
                    else if (roomItem.ItemData.InteractionType == InteractionType.LEGEND_BOX)
                    {
                        ItemLootBox.OpenLegendBox(session, roomItem, room);
                    }
                    else if (roomItem.ItemData.InteractionType == InteractionType.BADGE_BOX)
                    {
                        ItemLootBox.OpenBadgeBox(session, roomItem, room);
                    }
                    else if (roomItem.ItemData.InteractionType == InteractionType.GIFT_BANNER)
                    {
                        roomItem.Interactor.OnTrigger(session, roomItem, 0, true, false);
                    }
                    else if (roomItem.ItemData.InteractionType is InteractionType.CASE_MIEL or InteractionType.CASE_ATHENA or InteractionType.CASE_CELESTE or InteractionType.BAG_SAKURA or InteractionType.BAG_ATLANTA or InteractionType.BAG_KYOTO or InteractionType.BAG_NOEL or InteractionType.BAG_PORCELAINE)
                    {
                        ItemLootBox.OpenCaseOrBag(session, roomItem, room);
                    }
                }

                QuestManager.ProgressUserQuest(session, QuestType.FurniPlace, 0);
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
                var roomItem = new Item(userItem.Id, room.Id, userItem.BaseItemId, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, wallPos, room);
                if (room.RoomItemHandling.SetWallItem(session, roomItem))
                {
                    using (var dbClient = DatabaseManager.Connection)
                    {
                        ItemDao.UpdateRoomIdAndUserId(dbClient, itemId, room.Id, room.RoomData.OwnerId);

                        if (roomItem.ItemData.InteractionType == InteractionType.MOODLIGHT && room.MoodlightData == null)
                        {
                            var moodlightRow = ItemMoodlightDao.GetOne(dbClient, roomItem.Id);

                            var moodlightEnabled = moodlightRow != null && moodlightRow.Enabled;
                            var moodlightCurrentPreset = moodlightRow != null ? moodlightRow.CurrentPreset : 1;
                            var moodlightPresetOne = moodlightRow != null ? moodlightRow.PresetOne : "#000000,255,0";
                            var moodlightPresetTwo = moodlightRow != null ? moodlightRow.PresetTwo : "#000000,255,0";
                            var moodlightPresetThree = moodlightRow != null ? moodlightRow.PresetThree : "#000000,255,0";

                            room.MoodlightData = new MoodlightData(roomItem.Id, moodlightEnabled, moodlightCurrentPreset, moodlightPresetOne, moodlightPresetTwo, moodlightPresetThree);
                            roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
                        }
                    }

                    session.User.InventoryComponent.RemoveItem(itemId);
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

        position = ItemWallUtility.WallPositionCheck(wallPos);

        return position != "";
    }
}
