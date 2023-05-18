namespace WibboEmulator.Games.Items;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal static class ItemLootBox
{
    public static void OpenLootBox2022(GameClient session, Item present, Room room)
    {
        int pageId;
        var forceItem = 0;

        var probab = WibboEnvironment.GetRandomNumber(1, 20001);

        _ = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(1);
        var communCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(2);
        var epicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(3);
        var legendaryCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(4);

        if (probab <= 3 && legendaryCount <= 3)
        {
            pageId = 1635463734; // Legendaires
            WibboEnvironment.GetGame().GetLootManager().IncrementeRarityCounter(4);
        }
        else if (probab <= 33 && epicCount <= 30)
        {
            pageId = 1635463733; // Epic
            WibboEnvironment.GetGame().GetLootManager().IncrementeRarityCounter(3);
        }
        else if (probab <= 333 && communCount <= 300)
        {
            pageId = 1635463732; // Commun
            WibboEnvironment.GetGame().GetLootManager().IncrementeRarityCounter(2);
        }
        else if (probab <= 10933)
        {
            pageId = 15987; // 1wp
            forceItem = 23584;
        }
        else
        {
            pageId = 1635463731; // Basique
            WibboEnvironment.GetGame().GetLootManager().IncrementeRarityCounter(1);
        }

        EndOpenBox(session, present, room, pageId, forceItem);
    }

    public static void OpenLootBox(GameClient session, Item present, Room room)
    {
        var loots = WibboEnvironment.GetGame().GetLootManager().GetLoots(present.GetBaseItem().InteractionType);

        var pageId = 0;
        var forceItem = 0;

        foreach (var loot in loots.OrderBy(x => x.Probability).Where(x => x.Probability != 0))
        {
            if (WibboEnvironment.GetRandomNumber(1, loot.Probability) == loot.Probability)
            {
                if (loot.PageId > 0)
                {
                    pageId = loot.PageId;
                    forceItem = loot.ItemId;
                }

                if (loot.Category == "badge")
                {

                }

                break;
            }
        }

        EndOpenBox(session, present, room, pageId, forceItem);
    }

    private static readonly int ProbalilityLegendary = 5000;
    private static readonly int ProbalilityEpic = 500;
    private static readonly int ProbalilityCommun = 50;
    private static readonly int ProbalilityBasic = 5;

    public static void OpenExtrabox(GameClient session, Item present, Room room)
    {
        int pageId;
        var forceItem = 0;

        if (WibboEnvironment.GetRandomNumber(1, ProbalilityCommun) == ProbalilityCommun) //Common
        {
            pageId = 84641;
        }
        else if (WibboEnvironment.GetRandomNumber(1, ProbalilityBasic) == ProbalilityBasic) //Basic
        {
            pageId = 98747;
        }
        else //Basic
        {
            pageId = 894948;
        }

        EndOpenBox(session, present, room, pageId, forceItem);
    }

    public static void OpenDeluxeBox(GameClient session, Item present, Room room)
    {
        var forceItem = 0;

        int pageId;
        if (WibboEnvironment.GetRandomNumber(1, ProbalilityEpic) == ProbalilityEpic) //Epique
        {
            pageId = 1635463617;
        }
        else if (WibboEnvironment.GetRandomNumber(1, ProbalilityCommun) == ProbalilityCommun) //Commun
        {
            pageId = 1635463616;
        }
        else //Basic
        {
            pageId = 91700214;
        }

        EndOpenBox(session, present, room, pageId, forceItem);
    }

    public static void OpenBadgeBox(GameClient session, Item present, Room room)
    {
        //Présentoir et badge
        var pageId = 987987;

        var pageBadgeId = 18183;
        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageBadgeId, out var pageBadge);
        if (pageBadge == null)
        {
            return;
        }

        var badgeCode = pageBadge.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, pageBadge.Items.Count - 1)).Value.Badge;

        if (!string.IsNullOrEmpty(badgeCode) && !session.User.BadgeComponent.HasBadge(badgeCode))
        {
            session.User.BadgeComponent.GiveBadge(badgeCode, true);

            var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
            roomUserByUserId?.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("give.badge", roomUserByUserId.Client.Langue), badgeCode));
        }

        EndOpenBox(session, present, room, pageId, 0, badgeCode);
    }

    public static void OpenLegendBox(GameClient session, Item present, Room room)
    {
        var pageId = 0;
        var badgeCode = "";
        var lotType = "";
        var forceItem = 0;

        if (WibboEnvironment.GetRandomNumber(1, ProbalilityLegendary / 10) == ProbalilityLegendary / 10) //Legendaire
        {
            pageId = 14514;
            lotType = "Légendaire";
        }
        else if (WibboEnvironment.GetRandomNumber(1, 75) == 75) //Royal
        {
            pageId = 14515;
            lotType = "Royal";
            forceItem = 37951979;
        }
        else if (WibboEnvironment.GetRandomNumber(1, 30) == 30) //Royal
        {
            pageId = 14515;
            lotType = "Royal";
            forceItem = 70223722;
        }
        else if (WibboEnvironment.GetRandomNumber(1, ProbalilityEpic / 10) == ProbalilityEpic / 10) //Epique
        {
            pageId = 84641;
            lotType = "épique";
        }
        else if (WibboEnvironment.GetRandomNumber(1, 5) == 5) //Royal
        {
            pageId = 14515;
            lotType = "Royal";
            forceItem = 52394359;
        }
        else
        {
            pageId = 98747;
            lotType = "commun";
        }

        var pageBadgeId = 841878;

        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageBadgeId, out var pageBadge))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        foreach (var item in pageBadge.Items.OrderBy(a => Guid.NewGuid()).ToList())
        {
            if (session.User.BadgeComponent.HasBadge(item.Value.Badge))
            {
                continue;
            }

            badgeCode = item.Value.Badge;
            break;
        }

        var credits = WibboEnvironment.GetRandomNumber(100, 10000) * 1000;
        session.User.Credits += credits;
        session.SendPacket(new CreditBalanceComposer(session.User.Credits));

        var winwin = WibboEnvironment.GetRandomNumber(100, 1000);
        session.User.AchievementPoints += winwin;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserStatsDao.UpdateAchievementScore(dbClient, session.User.Id, winwin);
        }

        session.SendPacket(new AchievementScoreComposer(session.User.AchievementPoints));

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId != null)
        {
            session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
            room.SendPacket(new UserChangeComposer(roomUserByUserId, false));

            roomUserByUserId.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("item.legendboxlot", session.Langue), credits, winwin, badgeCode, lotType));
        }

        if (!string.IsNullOrEmpty(badgeCode))
        {
            session.User.BadgeComponent.GiveBadge(badgeCode, true);
        }

        EndOpenBox(session, present, room, pageId, forceItem);
    }

    private static void EndOpenBox(GameClient session, Item present, Room room, int pageId, int forceItem = 0, string extraData = "")
    {
        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out var page);
        if (page == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        ItemData lotData;
        if (forceItem == 0)
        {
            lotData = page.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, page.Items.Count - 1)).Value.Data;
        }
        else
        {
            lotData = page.GetItem(forceItem).Data;
        }

        if (lotData == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        room.RoomItemHandling.RemoveFurniture(session, present.Id);

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        if (lotData.IsRare)
        {
            LogLootBoxDao.Insert(dbClient, present.Data.InteractionType.ToString(), session.User.Id, present.Id, lotData.Id);
        }

        if (lotData.Amount >= 0)
        {
            ItemStatDao.UpdateAdd(dbClient, lotData.Id);
            lotData.Amount += 1;
        }

        ItemDao.UpdateBaseItem(dbClient, present.Id, lotData.Id);

        if (!string.IsNullOrEmpty(extraData))
        {
            ItemDao.UpdateExtradata(dbClient, present.Id, extraData);
        }

        if (!string.IsNullOrEmpty(extraData))
        {
            present.ExtraData = extraData;
        }

        present.BaseItem = lotData.Id;
        present.ResetBaseItem();

        var itemIsInRoom = true;

        if (present.Data.Type == 's')
        {
            if (!room.RoomItemHandling.SetFloorItem(session, present, present.X, present.Y, present.Rotation, true, false, true))
            {
                ItemDao.UpdateResetRoomId(dbClient, present.Id);

                itemIsInRoom = false;
            }
        }
        else
        {
            ItemDao.UpdateResetRoomId(dbClient, present.Id);

            itemIsInRoom = false;
        }

        session.SendPacket(new OpenGiftComposer(present.Data, present.ExtraData, present, itemIsInRoom));

        if (!itemIsInRoom)
        {
            session.User.InventoryComponent.TryAddItem(present);
        }
    }
}
