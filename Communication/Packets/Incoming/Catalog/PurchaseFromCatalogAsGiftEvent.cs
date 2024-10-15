namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Users;
using WibboEmulator.Utilities;

internal sealed class PurchaseFromCatalogAsGiftEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var pageId = packet.PopInt();
        var itemId = packet.PopInt();

        var extraData = packet.PopString(512);
        var giftUser = StringCharFilter.Escape(packet.PopString(16));
        var giftMessage = StringCharFilter.Escape(packet.PopString(140).Replace(Convert.ToChar(5), ' '));
        var boxSpriteId = packet.PopInt();
        var boxId = packet.PopInt();
        var ribbon = packet.PopInt();

        var showMyFace = packet.PopBoolean();

        if (!CatalogManager.TryGetPage(pageId, out var page))
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

        if (!ItemManager.GetGift(boxSpriteId, out var presentData) || presentData.InteractionType != InteractionType.GIFT)
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

        using var dbClient = DatabaseManager.Connection;

        var id = UserDao.GetIdByName(dbClient, giftUser);
        if (id == 0)
        {
            session.SendPacket(new GiftReceiverNotFoundComposer());
            return;
        }

        var user = UserManager.GetUserById(id);
        if (user == null)
        {
            session.SendPacket(new GiftReceiverNotFoundComposer());
            return;
        }

        if ((DateTime.Now - session.User.LastGiftPurchaseTime).TotalSeconds <= 15.0)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.buygift.flood", session.Language));

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

        if (!ItemUtility.TryProcessExtraData(item, session, ref extraData))
        {
            session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        var ed = (showMyFace ? session.User.Id : 0) + ";" + giftMessage + Convert.ToChar(5) + ribbon + Convert.ToChar(5) + boxId;

        var newItemId = ItemDao.Insert(dbClient, presentData.Id, user.Id, ed);

        ItemPresentDao.Insert(dbClient, newItemId, item.Data.Id, extraData);

        ItemDao.Delete(dbClient, newItemId);

        var giveItem = ItemFactory.CreateSingleItem(dbClient, presentData, user, ed, newItemId);
        if (giveItem != null)
        {
            var receiver = GameClientManager.GetClientByUserID(user.Id);
            receiver?.User.InventoryComponent.TryAddItem(giveItem);

            if (user.Id != session.User.Id && !string.IsNullOrWhiteSpace(giftMessage))
            {
                _ = AchievementManager.ProgressAchievement(session, "ACH_GiftGiver", 1);
                if (receiver != null)
                {
                    _ = AchievementManager.ProgressAchievement(receiver, "ACH_GiftReceiver", 1);
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
