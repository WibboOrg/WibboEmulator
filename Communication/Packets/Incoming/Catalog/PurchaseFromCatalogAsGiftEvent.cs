namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;
using WibboEmulator.Utilities;

internal class PurchaseFromCatalogAsGiftEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var pageId = packet.PopInt();
        var itemId = packet.PopInt();
        var data = packet.PopString();
        var giftUser = StringCharFilter.Escape(packet.PopString());
        var giftMessage = StringCharFilter.Escape(packet.PopString().Replace(Convert.ToChar(5), ' '));
        var spriteId = packet.PopInt();
        var ribbon = packet.PopInt();
        var colour = packet.PopInt();

        _ = packet.PopBoolean();

        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out var page))
        {
            return;
        }

        if (!page.Enabled || page.MinimumRank > session.User.Rank)
        {
            return;
        }

        if (!page.Items.TryGetValue(itemId, out var item))
        {
            return;
        }

        if (!ItemUtility.CanGiftItem(item))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetGift(spriteId, out var presentData) || presentData.InteractionType != InteractionType.GIFT)
        {
            return;
        }

        var totalCreditsCost = item.CostCredits;
        var totalPixelCost = item.CostDuckets;
        var totalDiamondCost = item.CostWibboPoints;
        var totalLimitCoinCost = item.CostLimitCoins;

        if (session.User.Credits < totalCreditsCost ||
            session.User.Duckets < totalPixelCost ||
            session.User.WibboPoints < totalDiamondCost ||
            session.User.LimitCoins < totalLimitCoinCost)
        {
            return;
        }

        var user = WibboEnvironment.GetUserByUsername(giftUser);
        if (user == null)
        {
            //session.SendPacket(new GiftWrappingErrorComposer());
            return;
        }

        if ((DateTime.Now - session.User.LastGiftPurchaseTime).TotalSeconds <= 15.0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buygift.flood", session.Langue));

            session.User.GiftPurchasingWarnings += 1;
            if (session.User.GiftPurchasingWarnings >= 3)
            {
                session.User.SessionGiftBlocked = true;
            }

            return;
        }

        if (session.User.SessionGiftBlocked)
        {
            return;
        }

        var ed = session.User.Id + ";" + giftMessage + Convert.ToChar(5) + ribbon + Convert.ToChar(5) + colour;
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        var newItemId = ItemDao.Insert(dbClient, presentData.Id, user.Id, ed);

        var itemExtraData = "";
        switch (item.Data.InteractionType)
        {
            case InteractionType.NONE:
                itemExtraData = "";
                break;

            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                var groupId = 0;
                if (!int.TryParse(data, out groupId))
                {
                    return;
                }

                if (groupId == 0)
                {
                    return;
                }

                Group groupItem;
                if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out groupItem))
                {
                    itemExtraData = "0;" + groupItem.Id;
                }

                break;

            case InteractionType.PET:

                var bits = data.Split('\n');

                if (bits.Length != 3)
                {
                    return;
                }

                var petName = bits[0];
                var race = bits[1];
                var color = bits[2];

                if (!int.TryParse(race, out _))
                {
                    return;
                }

                if (PetUtility.CheckPetName(petName))
                {
                    return;
                }

                if (race.Length > 2)
                {
                    return;
                }

                if (color.Length != 6)
                {
                    return;
                }

                _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetLover", 1);

                break;

            case InteractionType.FLOOR:
            case InteractionType.WALLPAPER:
            case InteractionType.LANDSCAPE:

                double number;
                if (string.IsNullOrEmpty(data))
                {
                    number = 0;
                }
                else
                {
                    _ = double.TryParse(data, out number);
                }

                itemExtraData = number.ToString().Replace(',', '.');
                break; // maintain extra data // todo: validate

            case InteractionType.POSTIT:
                itemExtraData = "FFFF33";
                break;

            case InteractionType.MOODLIGHT:
                itemExtraData = "1,1,1,#000000,255";
                break;

            case InteractionType.TROPHY:
                itemExtraData = session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + data;
                break;

            case InteractionType.MANNEQUIN:
                itemExtraData = "m;ch-210-1321.lg-285-92;Mannequin";
                break;

            case InteractionType.BADGE_TROC:
            {
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(data) || !WibboEnvironment.GetGame().GetCatalog().HasBadge(data))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    return;
                }

                if (!data.StartsWith("perso_"))
                {
                    session.User.BadgeComponent.RemoveBadge(data);
                }

                session.SendPacket(new BadgesComposer(session.User.BadgeComponent.BadgeList));

                itemExtraData = data;
                break;
            }

            case InteractionType.BADGE_DISPLAY:
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(data) || !session.User.BadgeComponent.HasBadge(data))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                itemExtraData = data + Convert.ToChar(9) + session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                break;

            default:
                itemExtraData = data;
                break;
        }

        UserPresentDao.Insert(dbClient, newItemId, item.Data.Id, itemExtraData);

        ItemDao.Delete(dbClient, newItemId);

        var giveItem = ItemFactory.CreateSingleItem(presentData, user, ed, newItemId);
        if (giveItem != null)
        {
            var receiver = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(user.Id);
            if (receiver != null)
            {
                _ = receiver.User.InventoryComponent.TryAddItem(giveItem);
                receiver.SendPacket(new FurniListNotificationComposer(giveItem.Id, 1));
                receiver.SendPacket(new PurchaseOKComposer());
                //Receiver.SendPacket(new FurniListUpdateComposer());
            }

            if (user.Id != session.User.Id && !string.IsNullOrWhiteSpace(giftMessage))
            {
                _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_GiftGiver", 1);
                if (receiver != null)
                {
                    _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(receiver, "ACH_GiftReceiver", 1);
                }
            }
        }

        session.SendPacket(new PurchaseOKComposer(item, presentData));

        if (item.CostCredits > 0)
        {
            session.User.Credits -= totalCreditsCost;
            session.SendPacket(new CreditBalanceComposer(session.User.Credits));
        }

        if (item.CostDuckets > 0)
        {
            session.User.Duckets -= totalPixelCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, session.User.Duckets));
        }

        if (item.CostWibboPoints > 0)
        {
            session.User.WibboPoints -= totalDiamondCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

            UserDao.UpdateRemovePoints(dbClient, session.User.Id, totalDiamondCost);
        }

        if (item.CostLimitCoins > 0)
        {
            session.User.LimitCoins -= totalLimitCoinCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.LimitCoins, 0, 55));

            UserDao.UpdateRemoveLimitCoins(dbClient, session.User.Id, totalLimitCoinCost);
        }

        session.User.LastGiftPurchaseTime = DateTime.Now;
    }
}
