namespace WibboEmulator.Games.Items;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Core.Settings;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Groups;

internal static class ItemBehaviourUtility
{
    public static void GenerateExtradata(Item item, ServerPacket message)
    {
        var itemData = item.ItemData;
        switch (itemData.InteractionType)
        {
            default:
                message.WriteInteger(item.Limited > 0 ? (int)ObjectDataKey.UNIQUE_SET + (int)ObjectDataKey.MAP_KEY : (int)ObjectDataKey.MAP_KEY);

                var totalSets = 1;
                if (itemData.RarityLevel > RaretyLevelType.None)
                {
                    totalSets++;
                }

                if (itemData.Amount >= 0)
                {
                    totalSets++;
                }

                message.WriteInteger(totalSets);

                if (itemData.RarityLevel > RaretyLevelType.None)
                {
                    message.WriteString("rarity");
                    message.WriteString(((int)itemData.RarityLevel).ToString());
                }

                if (itemData.Amount >= 0)
                {
                    message.WriteString("amount");
                    message.WriteString(itemData.Amount.ToString());
                }

                message.WriteString("state");
                message.WriteString((itemData.InteractionType is not InteractionType.TONER and not InteractionType.FOOTBALL_GATE) ? item.ExtraData : string.Empty);
                break;

            case InteractionType.EXCHANGE_TREE:
            case InteractionType.EXCHANGE_TREE_CLASSIC:
            case InteractionType.EXCHANGE_TREE_EPIC:
            case InteractionType.EXCHANGE_TREE_LEGEND:
                var days = 8;
                switch (itemData.InteractionType)
                {
                    case InteractionType.EXCHANGE_TREE:
                        days = 1;
                        break;
                    case InteractionType.EXCHANGE_TREE_CLASSIC:
                        days = 2;
                        break;
                    case InteractionType.EXCHANGE_TREE_EPIC:
                        days = 4;
                        break;
                    case InteractionType.EXCHANGE_TREE_LEGEND:
                        days = 8;
                        break;
                }

                var expireSeconds = days * 24 * 60 * 60;

                _ = int.TryParse(item.ExtraData, out var activateTime);

                var secondeLeft = activateTime + expireSeconds - WibboEnvironment.GetUnixTimestamp();

                var state = secondeLeft > 0 ? 10 - (int)Math.Ceiling((double)secondeLeft / expireSeconds * 10) : 10;

                message.WriteInteger(item.Limited > 0 ? (int)ObjectDataKey.UNIQUE_SET : (int)ObjectDataKey.LEGACY_KEY);
                message.WriteString(state.ToString());
                break;

            case InteractionType.TROPHY:
            case InteractionType.PHOTO:
            case InteractionType.BADGE_TROC:
                message.WriteInteger(item.Limited > 0 ? (int)ObjectDataKey.UNIQUE_SET : (int)ObjectDataKey.LEGACY_KEY);
                message.WriteString((itemData.InteractionType is not InteractionType.TONER and not InteractionType.FOOTBALL_GATE) ? item.ExtraData : string.Empty);
                break;

            case InteractionType.TROC_BANNER:
                if (!int.TryParse(item.ExtraData, out var bannerId) ||
                !BannerManager.TryGetBannerById(bannerId, out var banner))
                {
                    message.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                    message.WriteString(item.ExtraData);
                    break;
                }
                message.WriteInteger((int)ObjectDataKey.NUMBER_KEY);
                message.WriteInteger(2);
                message.WriteInteger(banner.Id);
                message.WriteInteger(banner.HaveLayer ? 1 : -1);
                break;

            case InteractionType.WALLPAPER:
            case InteractionType.FLOOR:
            case InteractionType.LANDSCAPE:
                message.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                message.WriteString(item.ExtraData);
                break;

            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                Group group = null;
                if (!GroupManager.TryGetGroup(item.GroupId, out group))
                {
                    message.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                    message.WriteString(item.ExtraData);
                }
                else
                {
                    message.WriteInteger((int)ObjectDataKey.STRING_KEY);
                    message.WriteInteger(5);
                    message.WriteString(item.ExtraData.Split(';')[0]);
                    message.WriteString(group.Id.ToString());
                    message.WriteString(group.Badge);
                    message.WriteString(GroupManager.GetColourCode(group.Colour1, true));
                    message.WriteString(GroupManager.GetColourCode(group.Colour2, false));
                }
                break;

            case InteractionType.HIGH_SCORE:
            case InteractionType.HIGH_SCORE_POINTS:
                message.WriteInteger((int)ObjectDataKey.HIGHSCORE_KEY); //Type

                message.WriteString(item.ExtraData);
                message.WriteInteger(2); //Type de victoire
                message.WriteInteger(0); //Type de duré


                message.WriteInteger((item.Scores.Count > 50) ? 50 : item.Scores.Count); //count

                foreach (var score in item.Scores.OrderByDescending(x => x.Value).Take(50))
                {
                    message.WriteInteger(score.Value); //score
                    message.WriteInteger(1); //(score.Key.Count); //count
                    //foreach(string UsernameScore in score.Key)
                    //Message.AppendString(UsernameScore);
                    message.WriteString(score.Key);
                }
                break;

            case InteractionType.LOVELOCK:
                if (item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                {
                    var eData = item.ExtraData.Split((char)5);
                    var i = 0;
                    message.WriteInteger((int)ObjectDataKey.STRING_KEY);
                    message.WriteInteger(eData.Length);
                    while (i < eData.Length)
                    {
                        message.WriteString(eData[i]);
                        i++;
                    }
                }
                else
                {
                    message.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                    message.WriteString("0");
                }
                break;

            case InteractionType.CRACKABLE:
                message.WriteInteger((int)ObjectDataKey.CRACKABLE_KEY); //Type

                _ = int.TryParse(item.ExtraData, out var clickNumber);

                message.WriteString(item.ExtraData);
                message.WriteInteger(clickNumber);
                message.WriteInteger(itemData.Modes - 1);
                break;

            case InteractionType.ADS_BACKGROUND:
                if (!string.IsNullOrEmpty(item.ExtraData))
                {
                    message.WriteInteger((int)ObjectDataKey.MAP_KEY);

                    var extraDatabackground = "state" + Convert.ToChar(9) + "0" + Convert.ToChar(9) + item.ExtraData;

                    extraDatabackground = extraDatabackground.Replace('=', Convert.ToChar(9));
                    message.WriteInteger(extraDatabackground.Split(Convert.ToChar(9)).Length / 2);

                    for (var i = 0; i <= extraDatabackground.Split(Convert.ToChar(9)).Length - 1; i++)
                    {
                        var data = extraDatabackground.Split(Convert.ToChar(9))[i];
                        message.WriteString(data);
                    }
                }
                else
                {
                    message.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                    message.WriteString("");
                }
                break;

            case InteractionType.LEGEND_BOX:
            case InteractionType.DELUXE_BOX:
            case InteractionType.EXTRA_BOX:
            case InteractionType.LOOTBOX_2022:
            case InteractionType.BADGE_BOX:
            {
                var lotName = "LootBox";
                switch (itemData.InteractionType)
                {
                    case InteractionType.LEGEND_BOX:
                        lotName = "LegendBox";
                        break;
                    case InteractionType.DELUXE_BOX:
                        lotName = "LootBox Deluxe";
                        break;
                    case InteractionType.BADGE_BOX:
                        lotName = "BadgeBox";
                        break;
                }

                message.WriteInteger((int)ObjectDataKey.MAP_KEY);
                message.WriteInteger(3);
                message.WriteString("MESSAGE");
                message.WriteString($"Bravo tu as reçu une {lotName} ! Ouvre-là pour y découvrir ton lot");
                message.WriteString("PURCHASER_NAME");
                message.WriteString("");
                message.WriteString("PURCHASER_FIGURE");
                message.WriteString("");
            }
            break;

            case InteractionType.PREMIUM_CLASSIC:
            case InteractionType.PREMIUM_EPIC:
            case InteractionType.PREMIUM_LEGEND:
            {
                var premiumName = "classique";
                switch (itemData.InteractionType)
                {
                    case InteractionType.PREMIUM_EPIC:
                        premiumName = "épique";
                        break;
                    case InteractionType.PREMIUM_LEGEND:
                        premiumName = "légende";
                        break;
                }

                message.WriteInteger((int)ObjectDataKey.MAP_KEY);
                message.WriteInteger(3);
                message.WriteString("MESSAGE");
                message.WriteString($"Bravo tu as reçu une {premiumName} box ! Ouvre-là pour reçevoir 31 jours de premium");
                message.WriteString("PURCHASER_NAME");
                message.WriteString("");
                message.WriteString("PURCHASER_FIGURE");
                message.WriteString("");
            }
            break;

            case InteractionType.GIFT_BANNER:
            {
                message.WriteInteger((int)ObjectDataKey.MAP_KEY);
                message.WriteInteger(3);
                message.WriteString("MESSAGE");
                message.WriteString($"Bravo tu as reçu une bannière box ! Ouvre-là pour reçevoir 1 bannière trocable aléatoire");
                message.WriteString("PURCHASER_NAME");
                message.WriteString("");
                message.WriteString("PURCHASER_FIGURE");
                message.WriteString("");
            }
            break;

            case InteractionType.GIFT:
            {
                if (!item.ExtraData.Contains(Convert.ToChar(5)))
                {
                    message.WriteInteger((int)ObjectDataKey.LEGACY_KEY);
                    message.WriteString(item.ExtraData);
                }
                else
                {
                    var giftData = item.ExtraData.Split(';', 2);
                    var giftUserId = int.Parse(giftData[0]);
                    var giftExtraData = giftData[1].Split(Convert.ToChar(5));
                    var giftMessage = giftExtraData[0];

                    var giftPurchaser = WibboEnvironment.GetUserById(giftUserId);
                    message.WriteInteger((int)ObjectDataKey.MAP_KEY);
                    message.WriteInteger(3); // Count
                    message.WriteString("MESSAGE");
                    message.WriteString(giftMessage);
                    message.WriteString("PURCHASER_NAME");
                    message.WriteString(giftPurchaser == null ? "" : giftPurchaser.Username);
                    message.WriteString("PURCHASER_FIGURE");
                    message.WriteString(giftPurchaser == null ? "" : giftPurchaser.Look);
                }
            }
            break;

            case InteractionType.MANNEQUIN:
                message.WriteInteger((int)ObjectDataKey.MAP_KEY);
                message.WriteInteger(3);
                if (item.ExtraData.Contains(';'))
                {
                    var stuff = item.ExtraData.Split(';');
                    message.WriteString("GENDER");
                    message.WriteString(stuff[0].Equals("M", StringComparison.CurrentCultureIgnoreCase) ? "M" : "F");
                    message.WriteString("FIGURE");
                    message.WriteString(stuff[1]);
                    message.WriteString("OUTFIT_NAME");
                    message.WriteString(stuff[2]);
                }
                else
                {
                    message.WriteString("GENDER");
                    message.WriteString("M");
                    message.WriteString("FIGURE");
                    message.WriteString("");
                    message.WriteString("OUTFIT_NAME");
                    message.WriteString("");
                }
                break;

            case InteractionType.TONER:
                if (item.ExtraData.Contains(','))
                {
                    message.WriteInteger(5);
                    message.WriteInteger(4);
                    message.WriteInteger(item.ExtraData.StartsWith("on") ? 1 : 0);
                    message.WriteInteger(int.Parse(item.ExtraData.Split(',')[1]));
                    message.WriteInteger(int.Parse(item.ExtraData.Split(',')[2]));
                    message.WriteInteger(int.Parse(item.ExtraData.Split(',')[3]));
                }
                else
                {
                    message.WriteInteger(0);
                    message.WriteString(string.Empty);
                }
                break;

            case InteractionType.BADGE_DISPLAY:
                message.WriteInteger((int)ObjectDataKey.STRING_KEY);
                message.WriteInteger(4);

                if (item.ExtraData.Contains(Convert.ToChar(9).ToString()))
                {
                    var badgeData = item.ExtraData.Split(Convert.ToChar(9));

                    message.WriteString("0");//No idea
                    message.WriteString(badgeData[0]);//Badge name
                    message.WriteString(badgeData[1]);//Owner
                    message.WriteString(badgeData[2]);//Date
                }
                else
                {
                    message.WriteString("0");//No idea
                    message.WriteString(item.ExtraData);//Badge name
                    message.WriteString("");//Owner
                    message.WriteString("");//Date
                }
                break;

            case InteractionType.TV_YOUTUBE:
                message.WriteInteger((int)ObjectDataKey.MAP_KEY);
                message.WriteInteger(2);
                message.WriteString("THUMBNAIL_URL");
                message.WriteString(string.IsNullOrEmpty(item.ExtraData) ? "" : "https://" + SettingsManager.GetData<string>("cdn.url") + "/youtubethumbnail.php?videoid=" + item.ExtraData);
                message.WriteString("VideoId");
                message.WriteString(item.ExtraData);
                break;
        }

        if (item.Limited > 0)
        {
            message.WriteInteger(item.Limited);
            message.WriteInteger(item.LimitedStack);
        }
    }

    public static void GenerateWallExtradata(Item item, ServerPacket message)
    {
        switch (item.ItemData.InteractionType)
        {
            default:
                message.WriteString(item.ExtraData);
                break;

            case InteractionType.POSTIT:
                message.WriteString(item.ExtraData.Contains(' ') ? item.ExtraData.Split(' ')[0] : "");
                break;
        }
    }
}

public enum ObjectDataKey
{
    LEGACY_KEY = 0,
    MAP_KEY = 1,
    STRING_KEY = 2,
    VOTE_KEY = 3,
    EMPTY_KEY = 4,
    NUMBER_KEY = 5,
    HIGHSCORE_KEY = 6,
    CRACKABLE_KEY = 7,
    UNIQUE_SET = 256
}
