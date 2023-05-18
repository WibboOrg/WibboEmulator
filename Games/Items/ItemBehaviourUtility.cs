namespace WibboEmulator.Games.Items;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Games.Groups;

internal static class ItemBehaviourUtility
{
    public static void GenerateExtradata(Item item, ServerPacket message)
    {
        var itemData = item.GetBaseItem();

        message.WriteInteger(ItemCategory(item));

        switch (itemData.InteractionType)
        {
            default:
                message.WriteInteger(item.Limited > 0 ? 256 + 1 : 1);

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
                var days = 31;
                switch (itemData.InteractionType)
                {
                    case InteractionType.EXCHANGE_TREE:
                        days = 3;
                        break;
                    case InteractionType.EXCHANGE_TREE_CLASSIC:
                        days = 7;
                        break;
                    case InteractionType.EXCHANGE_TREE_EPIC:
                        days = 14;
                        break;
                    case InteractionType.EXCHANGE_TREE_LEGEND:
                        days = 31;
                        break;
                }

                var expireSeconds = days * 24 * 60 * 60;

                _ = int.TryParse(item.ExtraData, out var activateTime);

                var secondeLeft = activateTime + expireSeconds - WibboEnvironment.GetUnixTimestamp();

                var state = secondeLeft > 0 ? 10 - (int)Math.Ceiling((double)secondeLeft / expireSeconds * 10) : 10;

                message.WriteInteger(item.Limited > 0 ? 256 : 0);
                message.WriteString(state.ToString());
                break;

            case InteractionType.TROPHY:
            case InteractionType.PHOTO:
                message.WriteInteger(item.Limited > 0 ? 256 : 0);
                message.WriteString((itemData.InteractionType is not InteractionType.TONER and not InteractionType.FOOTBALL_GATE) ? item.ExtraData : string.Empty);
                break;

            case InteractionType.WALLPAPER:
                message.WriteInteger(0);
                message.WriteString(item.ExtraData);

                break;
            case InteractionType.FLOOR:
                message.WriteInteger(0);
                message.WriteString(item.ExtraData);
                break;

            case InteractionType.LANDSCAPE:
                message.WriteInteger(0);
                message.WriteString(item.ExtraData);
                break;

            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                Group group = null;
                if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(item.GroupId, out group))
                {
                    message.WriteInteger(0);
                    message.WriteString(item.ExtraData);
                }
                else
                {
                    message.WriteInteger(2);
                    message.WriteInteger(5);
                    message.WriteString(item.ExtraData.Split(new char[1] { ';' })[0]);
                    message.WriteString(group.Id.ToString());
                    message.WriteString(group.Badge);
                    message.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                    message.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                }
                break;

            case InteractionType.HIGH_SCORE:
            case InteractionType.HIGH_SCORE_POINTS:
                message.WriteInteger(6); //Type

                message.WriteString(item.ExtraData);
                message.WriteInteger(2); //Type de victoire
                message.WriteInteger(0); //Type de duré


                message.WriteInteger((item.Scores.Count > 20) ? 20 : item.Scores.Count); //count

                foreach (var score in item.Scores.OrderByDescending(x => x.Value).Take(20))
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
                    message.WriteInteger(2);
                    message.WriteInteger(eData.Length);
                    while (i < eData.Length)
                    {
                        message.WriteString(eData[i]);
                        i++;
                    }
                }
                else
                {
                    message.WriteInteger(0);
                    message.WriteString("0");
                }
                break;

            case InteractionType.CRACKABLE:
                message.WriteInteger(7); //Type

                _ = int.TryParse(item.ExtraData, out var clickNumber);

                message.WriteString(item.ExtraData);
                message.WriteInteger(clickNumber);
                message.WriteInteger(itemData.Modes - 1);
                break;

            case InteractionType.ADS_BACKGROUND:
                if (!string.IsNullOrEmpty(item.ExtraData))
                {
                    message.WriteInteger(1);

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
                    message.WriteInteger(0);
                    message.WriteString("");
                }
                break;

            case InteractionType.LEGEND_BOX:
            case InteractionType.DELUXE_BOX:
            case InteractionType.EXTRA_BOX:
            case InteractionType.LOOTBOX_2022:
            case InteractionType.BADGE_BOX:
            {
                var lotName = "RareBox";
                switch (itemData.InteractionType)
                {
                    case InteractionType.LEGEND_BOX:
                        lotName = "LegendBox";
                        break;
                    case InteractionType.DELUXE_BOX:
                        lotName = "RareBox Deluxe";
                        break;
                    case InteractionType.BADGE_BOX:
                        lotName = "BadgeBox";
                        break;
                }

                message.WriteInteger(1);
                message.WriteInteger(3);
                message.WriteString("MESSAGE");
                message.WriteString($"Bravo tu as reçu une {lotName} ! Ouvre-là pour y découvrir ton lot");
                message.WriteString("PURCHASER_NAME");
                message.WriteString("Wibbo");
                message.WriteString("PURCHASER_FIGURE");
                message.WriteString("");
            }
            break;

            case InteractionType.GIFT:
            {
                if (!item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                {
                    message.WriteInteger(0);
                    message.WriteString(item.ExtraData);
                }
                else
                {

                    var extraData = item.ExtraData.Split(Convert.ToChar(5));
                    var style = (int.Parse(item.ExtraData.Split(new char[1] { '\x0005' })[1]) * 1000) + int.Parse(item.ExtraData.Split(new char[1] { '\x0005' })[2]);

                    var purchaser = WibboEnvironment.GetUserById(int.Parse(item.ExtraData.Split(new char[1] { ';' })[0]));
                    message.WriteInteger(1);
                    message.WriteInteger(6);
                    message.WriteString("EXTRA_PARAM");
                    message.WriteString("");
                    message.WriteString("MESSAGE");
                    message.WriteString(item.ExtraData.Split(new char[1] { ';' })[1].Split(new char[1] { '\x0005' })[0]);
                    message.WriteString("PURCHASER_NAME");
                    message.WriteString(purchaser == null ? "" : purchaser.Username);
                    message.WriteString("PURCHASER_FIGURE");
                    message.WriteString(purchaser == null ? "" : purchaser.Look);
                    message.WriteString("PRODUCT_CODE");
                    message.WriteString(itemData.SpriteId.ToString());
                    message.WriteString("state");
                    message.WriteString(style.ToString());
                }
            }
            break;

            case InteractionType.MANNEQUIN:
                message.WriteInteger(1);
                message.WriteInteger(3);
                if (item.ExtraData.Contains(';'))
                {
                    var stuff = item.ExtraData.Split(new char[1] { ';' });
                    message.WriteString("GENDER");
                    message.WriteString(stuff[0].ToUpper() == "M" ? "M" : "F");
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
                    message.WriteInteger(int.Parse(item.ExtraData.Split(new char[1] { ',' })[1]));
                    message.WriteInteger(int.Parse(item.ExtraData.Split(new char[1] { ',' })[2]));
                    message.WriteInteger(int.Parse(item.ExtraData.Split(new char[1] { ',' })[3]));
                }
                else
                {
                    message.WriteInteger(0);
                    message.WriteString(string.Empty);
                }
                break;

            case InteractionType.BADGE_DISPLAY:
            case InteractionType.BADGE_TROC:
                message.WriteInteger(2);
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
                message.WriteInteger(1);
                message.WriteInteger(2);
                message.WriteString("THUMBNAIL_URL");
                message.WriteString(string.IsNullOrEmpty(item.ExtraData) ? "" : "https://cdn.wibbo.org/youtubethumbnail.php?videoid=" + item.ExtraData);
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
        switch (item.GetBaseItem().InteractionType)
        {
            default:
                message.WriteString(item.ExtraData);
                break;

            case InteractionType.POSTIT:
                message.WriteString(item.ExtraData.Contains(' ') ? item.ExtraData.Split(' ')[0] : "");
                break;
        }
    }

    public static int ItemCategory(Item item) => item.GetBaseItem().InteractionType switch
    {
        InteractionType.GIFT or InteractionType.LEGEND_BOX or InteractionType.BADGE_BOX or InteractionType.LOOTBOX_2022 or InteractionType.DELUXE_BOX or InteractionType.EXTRA_BOX => 9,
        InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE => 17,
        InteractionType.LANDSCAPE => 4,
        InteractionType.FLOOR => 3,
        InteractionType.WALLPAPER => 2,
        InteractionType.TROPHY => 11,
        _ => 1,
    };
}
