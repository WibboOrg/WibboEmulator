using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Users;

namespace WibboEmulator.Game.Items
{
    internal static class ItemBehaviourUtility
    {
        public static void GenerateExtradata(Item item, ServerPacket message)
        {
            ItemData itemData = item.GetBaseItem();

            message.WriteInteger(ItemCategory(item));

            switch (itemData.InteractionType)
            {
                default:
                    message.WriteInteger(item.Limited > 0 ? 256 + 1 : 1);

                    int totalSets = 1;
                    if (itemData.RarityLevel > 0) totalSets++;
                    if (itemData.Amount >= 0) totalSets++;

                    message.WriteInteger(totalSets);

                    if (itemData.RarityLevel > 0)
                    {
                        message.WriteString("rarity");
                        message.WriteString(itemData.RarityLevel.ToString());
                    }
                    
                    if (itemData.Amount >= 0)
                    {
                        message.WriteString("amount");
                        message.WriteString(itemData.Amount.ToString());
                    }

                    message.WriteString("state");
                    message.WriteString((itemData.InteractionType != InteractionType.TONER && itemData.InteractionType != InteractionType.FBGATE) ? item.ExtraData : string.Empty);
                    break;

                case InteractionType.TROPHY:
                case InteractionType.PHOTO:
                    message.WriteInteger(item.Limited > 0 ? 256 : 0);
                    message.WriteString((itemData.InteractionType != InteractionType.TONER && itemData.InteractionType != InteractionType.FBGATE) ? item.ExtraData : string.Empty);
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
                    Group Group = null;
                    if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(item.GroupId, out Group))
                    {
                        message.WriteInteger(0);
                        message.WriteString(item.ExtraData);
                    }
                    else
                    {
                        message.WriteInteger(2);
                        message.WriteInteger(5);
                        message.WriteString(item.ExtraData.Split(new char[1] { ';' })[0]);
                        message.WriteString(Group.Id.ToString());
                        message.WriteString(Group.Badge);
                        message.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour1, true));
                        message.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour2, false));
                    }
                    break;

                case InteractionType.HIGHSCORE:
                case InteractionType.HIGHSCOREPOINTS:
                    message.WriteInteger(6); //Type

                    message.WriteString(item.ExtraData);
                    message.WriteInteger(2); //Type de victoire
                    message.WriteInteger(0); //Type de duré


                    message.WriteInteger((item.Scores.Count > 20) ? 20 : item.Scores.Count); //count

                    foreach (KeyValuePair<string, int> score in item.Scores.OrderByDescending(x => x.Value).Take(20))
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
                        string[] EData = item.ExtraData.Split((char)5);
                        int I = 0;
                        message.WriteInteger(2);
                        message.WriteInteger(EData.Length);
                        while (I < EData.Length)
                        {
                            message.WriteString(EData[I]);
                            I++;
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

                    int.TryParse(item.ExtraData, out int ClickNumber);

                    message.WriteString(item.ExtraData);
                    message.WriteInteger(ClickNumber);
                    message.WriteInteger(itemData.Modes - 1); //Type de duré
                    break;

                case InteractionType.ADS_BACKGROUND:
                    if (!string.IsNullOrEmpty(item.ExtraData))
                    {
                        message.WriteInteger(1);

                        string ExtraDatabackground = "state" + Convert.ToChar(9) + "0" + Convert.ToChar(9) + item.ExtraData;

                        ExtraDatabackground = ExtraDatabackground.Replace('=', Convert.ToChar(9));
                        message.WriteInteger(ExtraDatabackground.Split(Convert.ToChar(9)).Length / 2);

                        for (int i = 0; i <= ExtraDatabackground.Split(Convert.ToChar(9)).Length - 1; i++)
                        {
                            string Data = ExtraDatabackground.Split(Convert.ToChar(9))[i];
                            message.WriteString(Data);
                        }
                    }
                    else
                    {
                        message.WriteInteger(0);
                        message.WriteString("");
                    }
                    break;

                case InteractionType.LEGENDBOX:
                case InteractionType.DELUXEBOX:
                case InteractionType.EXTRABOX:
                case InteractionType.LOOTBOX2022:
                case InteractionType.BADGEBOX:
                    {
                        string LotName = "RareBox";
                        switch (itemData.InteractionType)
                        {
                            case InteractionType.LEGENDBOX:
                                LotName = "LegendBox";
                                break;
                            case InteractionType.DELUXEBOX:
                                LotName = "RareBox Deluxe";
                                break;
                            case InteractionType.BADGEBOX:
                                LotName = "BadgeBox";
                                break;
                        }

                    message.WriteInteger(1);
                        message.WriteInteger(3);
                        message.WriteString("MESSAGE");
                        message.WriteString($"Bravo tu as reçu une {LotName} ! Ouvre-là pour y découvrir ton lot");
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

                            string[] ExtraData = item.ExtraData.Split(Convert.ToChar(5));
                            int Style = int.Parse(item.ExtraData.Split(new char[1] { '\x0005' })[1]) * 1000 + int.Parse(item.ExtraData.Split(new char[1] { '\x0005' })[2]);

                            User Purchaser = WibboEnvironment.GetUserById(int.Parse(item.ExtraData.Split(new char[1] { ';' })[0]));
                            message.WriteInteger(1);
                            message.WriteInteger(6);
                            message.WriteString("EXTRA_PARAM");
                            message.WriteString("");
                            message.WriteString("MESSAGE");
                            message.WriteString(item.ExtraData.Split(new char[1] { ';' })[1].Split(new char[1] { '\x0005' })[0]);
                            message.WriteString("PURCHASER_NAME");
                            message.WriteString(Purchaser == null ? "" : Purchaser.Username);
                            message.WriteString("PURCHASER_FIGURE");
                            message.WriteString(Purchaser == null ? "" : Purchaser.Look);
                            message.WriteString("PRODUCT_CODE");
                            message.WriteString(itemData.SpriteId.ToString());
                            message.WriteString("state");
                            message.WriteString(Style.ToString());
                        }
                    }
                    break;

                case InteractionType.MANNEQUIN:
                    message.WriteInteger(1);
                    message.WriteInteger(3);
                    if (item.ExtraData.Contains(';'))
                    {
                        string[] Stuff = item.ExtraData.Split(new char[1] { ';' });
                        message.WriteString("GENDER");
                        message.WriteString(Stuff[0].ToUpper() == "M" ? "M" : "F");
                        message.WriteString("FIGURE");
                        message.WriteString(Stuff[1]);
                        message.WriteString("OUTFIT_NAME");
                        message.WriteString(Stuff[2]);
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
                        string[] BadgeData = item.ExtraData.Split(Convert.ToChar(9));

                        message.WriteString("0");//No idea
                        message.WriteString(BadgeData[0]);//Badge name
                        message.WriteString(BadgeData[1]);//Owner
                        message.WriteString(BadgeData[2]);//Date
                    }
                    else
                    {
                        message.WriteString("0");//No idea
                        message.WriteString(item.ExtraData);//Badge name
                        message.WriteString("");//Owner
                        message.WriteString("");//Date
                    }
                    break;

                case InteractionType.TVYOUTUBE:
                    message.WriteInteger(1);
                    message.WriteInteger(2);
                    message.WriteString("THUMBNAIL_URL");
                    message.WriteString((string.IsNullOrEmpty(item.ExtraData)) ? "" : "https://cdn.wibbo.org/youtubethumbnail.php?videoid=" + item.ExtraData);
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
                    message.WriteString((item.ExtraData.Contains(' ')) ? item.ExtraData.Split(' ')[0] : "");
                    break;
            }
        }

        public static int ItemCategory(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.GIFT:
                case InteractionType.LEGENDBOX:
                case InteractionType.BADGEBOX:
                case InteractionType.LOOTBOX2022:
                case InteractionType.DELUXEBOX:
                case InteractionType.EXTRABOX:
                    return 9;
                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    return 17;
                case InteractionType.LANDSCAPE:
                    return 4;
                case InteractionType.FLOOR:
                    return 3;
                case InteractionType.WALLPAPER:
                    return 2;
                case InteractionType.TROPHY:
                    return 11;
                default:
                    return 1;
            }
        }
    }
}