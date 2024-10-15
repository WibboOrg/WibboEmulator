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

    public void Parse(GameClient Session, ClientPacket packet)
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

        if (!page.Enabled || !page.HavePermission(Session.User))
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

        if (page.IsPremium && Session.User.Rank < 2)
        {
            Session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        var totalCreditsCost = item.CostCredits;
        var totalPixelCost = item.CostDuckets;
        var totalDiamondCost = item.CostWibboPoints;
        var totalLimitCoinCost = item.CostLimitCoins;

        if (Session.User.Credits < totalCreditsCost ||
            Session.User.Duckets < totalPixelCost ||
            Session.User.WibboPoints < totalDiamondCost ||
            Session.User.LimitCoins < totalLimitCoinCost)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        var id = UserDao.GetIdByName(dbClient, giftUser);
        if (id == 0)
        {
            Session.SendPacket(new GiftReceiverNotFoundComposer());
            return;
        }

        var user = UserManager.GetUserById(id);
        if (user == null)
        {
            Session.SendPacket(new GiftReceiverNotFoundComposer());
            return;
        }

        if ((DateTime.Now - Session.User.LastGiftPurchaseTime).TotalSeconds <= 15.0)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.buygift.flood", Session.Language));

            Session.User.GiftPurchasingWarnings += 1;
            if (Session.User.GiftPurchasingWarnings >= 3)
            {
                Session.User.SessionGiftBlocked = true;
            }

            return;
        }

        if (Session.User.SessionGiftBlocked)
        {
            return;
        }

        if (!ItemUtility.TryProcessExtraData(item, Session, ref extraData))
        {
            Session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        var ed = (showMyFace ? Session.User.Id : 0) + ";" + giftMessage + Convert.ToChar(5) + ribbon + Convert.ToChar(5) + boxId;

        var newItemId = ItemDao.Insert(dbClient, presentData.Id, user.Id, ed);

        ItemPresentDao.Insert(dbClient, newItemId, item.Data.Id, extraData);

        ItemDao.Delete(dbClient, newItemId);

        var giveItem = ItemFactory.CreateSingleItem(dbClient, presentData, user, ed, newItemId);
        if (giveItem != null)
        {
            var receiver = GameClientManager.GetClientByUserID(user.Id);
            receiver?.User.InventoryComponent.TryAddItem(giveItem);

            if (user.Id != Session.User.Id && !string.IsNullOrWhiteSpace(giftMessage))
            {
                _ = AchievementManager.ProgressAchievement(Session, "ACH_GiftGiver", 1);
                if (receiver != null)
                {
                    _ = AchievementManager.ProgressAchievement(receiver, "ACH_GiftReceiver", 1);
                }
            }
        }

        Session.SendPacket(new PurchaseOKComposer(item, presentData));

        if (item.CostCredits > 0)
        {
            Session.User.Credits -= totalCreditsCost;
            Session.SendPacket(new CreditBalanceComposer(Session.User.Credits));
        }

        if (item.CostDuckets > 0)
        {
            Session.User.Duckets -= totalPixelCost;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.Duckets, Session.User.Duckets));
        }

        if (item.CostWibboPoints > 0)
        {
            Session.User.WibboPoints -= totalDiamondCost;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.WibboPoints, 0, 105));

            UserDao.UpdateRemovePoints(dbClient, Session.User.Id, totalDiamondCost);
        }

        if (item.CostLimitCoins > 0)
        {
            Session.User.LimitCoins -= totalLimitCoinCost;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.LimitCoins, 0, 55));

            UserDao.UpdateRemoveLimitCoins(dbClient, Session.User.Id, totalLimitCoinCost);
        }

        Session.User.LastGiftPurchaseTime = DateTime.Now;
    }
}
