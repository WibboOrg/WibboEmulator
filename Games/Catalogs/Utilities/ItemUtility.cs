namespace WibboEmulator.Games.Catalogs.Utilities;

using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;

public static class ItemUtility
{
    public static bool CanGiftItem(CatalogItem item)
    {
        if (item.Data.InteractionType == InteractionType.TROPHY)
        {
            return true;
        }

        if (!item.Data.AllowGift || item.IsLimited || item.Amount > 1 || item.Data.InteractionType == InteractionType.EXCHANGE ||
            item.Data.InteractionType == InteractionType.BADGE || (item.Data.Type != ItemType.S && item.Data.Type != ItemType.I) || item.CostWibboPoints > 0 || item.CostLimitCoins > 0 ||
            item.Data.InteractionType == InteractionType.TELEPORT || item.Data.InteractionType == InteractionType.TELEPORT_ARROW)
        {
            return false;
        }

        if (item.Data.IsRare || item.Data.RarityLevel > RaretyLevelType.None)
        {
            return false;
        }

        if (item.Data.InteractionType == InteractionType.PET)
        {
            return false;
        }

        return true;
    }

    public static bool CanSelectAmount(CatalogItem item)
    {
        if (item.IsLimited || item.Amount > 1 || item.Data.InteractionType == InteractionType.EXCHANGE || !item.HaveOffer || item.Data.InteractionType == InteractionType.BADGE)
        {
            return false;
        }

        return true;
    }

    public static int GetSaddleId(int saddle) => saddle switch
    {
        10 => 7544143,
        _ => 2804,
    };

    public static bool IsRare(Item item)
    {
        if (item.Data.InteractionType == InteractionType.EXCHANGE)
        {
            return false;
        }

        if (item.Limited > 0)
        {
            return true;
        }

        if (item.Data.IsRare || item.Data.RarityLevel > RaretyLevelType.None)
        {
            return true;
        }

        return false;
    }

    public static bool TryProcessExtraData(CatalogItem item, GameClient session, ref string extraData)
    {
        switch (item.Data.InteractionType)
        {
            case InteractionType.TROC_BANNER:
                if (!int.TryParse(extraData, out var bannerId))
                {
                    return false;
                }

                if (!BannerManager.TryGetBannerById(bannerId, out var banner))
                {
                    return false;
                }

                extraData = banner.Id.ToString();
                break;
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
                    return false;
                }

                Group group;
                if (!GroupManager.TryGetGroup(groupId, out group))
                {
                    return false;
                }

                extraData = "0;" + group.Id;
                break;

            case InteractionType.PET:
                if (string.IsNullOrEmpty(extraData) || !extraData.Contains('\n'))
                {
                    return false;
                }

                var bits = extraData.Split('\n');

                if (bits.Length < 3)
                {
                    return false;
                }

                var petName = bits[0];
                var race = bits[1];
                var color = bits[2];

                if (!int.TryParse(race, out _) || color.Length != 6 || race.Length > 2 || !PetUtility.CheckPetName(petName))
                {
                    return false;
                }

                _ = AchievementManager.ProgressAchievement(session, "ACH_PetLover", 1);

                break;

            case InteractionType.FLOOR:
            case InteractionType.WALLPAPER:
            case InteractionType.LANDSCAPE:

                _ = double.TryParse(extraData, out var number);

                extraData = number.ToString();
                break;

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
                if (BadgeManager.HaveNotAllowed(extraData) || !CatalogManager.HasBadge(extraData) || !session.User.BadgeComponent.HasBadge(extraData))
                {
                    session.SendNotification(LanguageManager.TryGetValue("notif.buybadgedisplay.error", session.Language));
                    return false;
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
                    session.SendNotification(LanguageManager.TryGetValue("notif.buybadgedisplay.error", session.Language));
                    return false;
                }

                extraData = extraData + Convert.ToChar(9) + session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                break;

            case InteractionType.BADGE:
            {
                if (session.User.BadgeComponent.HasBadge(item.Badge))
                {
                    session.SendNotification(LanguageManager.TryGetValue("notif.buybadge.error", session.Language));
                    return false;
                }
                break;
            }
            default:
                extraData = "";
                break;
        }

        return true;
    }
}
