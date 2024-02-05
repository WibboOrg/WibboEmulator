namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;
using WibboEmulator.Utilities;

internal sealed class PurchaseFromCatalogAsGiftEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var pageId = packet.PopInt();
        var itemId = packet.PopInt();

        var extraData = packet.PopString();
        var giftUser = StringCharFilter.Escape(packet.PopString(16));
        var giftMessage = StringCharFilter.Escape(packet.PopString().Replace(Convert.ToChar(5), ' '));
        var spriteId = packet.PopInt();
        var boxId = packet.PopInt();
        var ribbon = packet.PopInt();

        var showMyFace = packet.PopBoolean();

        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out var page))
        {
            return;
        }

        if (!page.Enabled || !page.HavePermission(session.User))
        {
            return;
        }

        if (!page.Items.TryGetValue(itemId, out var item))
        {
            if (page.ItemOffers.TryGetValue(itemId, out var value))
            {
                item = value;
                if (item == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        if (!ItemUtility.CanGiftItem(item))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetGift(spriteId, out var presentData) || presentData.InteractionType != InteractionType.GIFT)
        {
            return;
        }

        if (page.IsPremium && session.User.Rank < 2)
        {
            session.SendPacket(new PurchaseErrorComposer());
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

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        var id = UserDao.GetIdByName(dbClient, giftUser);
        if (id == 0)
        {
            session.SendPacket(new GiftReceiverNotFoundComposer());
            return;
        }

        var user = WibboEnvironment.GetUserById(id);
        if (user == null)
        {
            session.SendPacket(new GiftReceiverNotFoundComposer());
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

        var ed = (showMyFace ? session.User.Id : 0) + ";" + giftMessage + Convert.ToChar(5) + ribbon + Convert.ToChar(5) + boxId;

        var newItemId = ItemDao.Insert(dbClient, presentData.Id, user.Id, ed);

        switch (item.Data.InteractionType)
        {
            case InteractionType.WIRED_ITEM:
            case InteractionType.NONE:
                extraData = "";
                break;

            case InteractionType.EXCHANGE_TREE:
            case InteractionType.EXCHANGE_TREE_CLASSIC:
            case InteractionType.EXCHANGE_TREE_EPIC:
            case InteractionType.EXCHANGE_TREE_LEGEND:
                extraData = WibboEnvironment.GetUnixTimestamp().ToString();
                break;
            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                int groupId;
                if (!int.TryParse(extraData, out groupId))
                {
                    session.SendPacket(new PurchaseErrorComposer());
                    return;
                }

                Group group;
                if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out group))
                {
                    session.SendPacket(new PurchaseErrorComposer());
                    return;
                }

                extraData = "0;" + group.Id;
                break;

            case InteractionType.PET:
                var bits = extraData.Split('\n');
                var petName = bits[0];
                var race = bits[1];
                var color = bits[2];

                if (!int.TryParse(race, out _) || color.Length != 6 || race.Length > 2 || !PetUtility.CheckPetName(petName))
                {
                    session.SendPacket(new PurchaseErrorComposer());
                    return;
                }

                _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetLover", 1);

                break;

            case InteractionType.FLOOR:
            case InteractionType.WALLPAPER:
            case InteractionType.LANDSCAPE:

                double number;
                if (string.IsNullOrEmpty(extraData))
                {
                    number = 0;
                }
                else
                {
                    _ = double.TryParse(extraData, out number);
                }

                extraData = number.ToString().Replace(',', '.');
                break; // maintain extra data // todo: validate

            case InteractionType.POSTIT:
                extraData = "FFFF33";
                break;

            case InteractionType.MOODLIGHT:
                extraData = "1,1,1,#000000,255";
                break;

            case InteractionType.TROPHY:
                extraData = session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + extraData;
                break;

            case InteractionType.MANNEQUIN:
                extraData = "m;ch-210-1321.lg-285-92;Mannequin";
                break;

            case InteractionType.BADGE_TROC:
            {
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(extraData) || !WibboEnvironment.GetGame().GetCatalog().HasBadge(extraData) || !session.User.BadgeComponent.HasBadge(extraData))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseErrorComposer());
                    return;
                }

                if (!extraData.StartsWith("perso_"))
                {
                    session.User.BadgeComponent.RemoveBadge(extraData);
                }

                break;
            }

            case InteractionType.BADGE_DISPLAY:
                if (!session.User.BadgeComponent.HasBadge(extraData))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseErrorComposer());
                    return;
                }

                extraData = extraData + Convert.ToChar(9) + session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                break;

            case InteractionType.BADGE:
            {
                if (session.User.BadgeComponent.HasBadge(item.Badge))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadge.error", session.Langue));
                    session.SendPacket(new PurchaseErrorComposer());
                    return;
                }
                break;
            }
            default:
                extraData = "";
                break;
        }

        ItemPresentDao.Insert(dbClient, newItemId, item.Data.Id, extraData);

        ItemDao.Delete(dbClient, newItemId);

        var giveItem = ItemFactory.CreateSingleItem(dbClient, presentData, user, ed, newItemId);
        if (giveItem != null)
        {
            var receiver = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(user.Id);
            receiver?.User.InventoryComponent.TryAddItem(giveItem);

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
