namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.BuildersClub;
using WibboEmulator.Communication.Packets.Outgoing.Economy;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Settings;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Helps;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users.Authentificator;
using WibboEmulator.Utilities;

internal sealed class SSOTicketEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User != null)
        {
            return;
        }

        var ssoTicket = packet.PopString();
        _ = packet.PopInt();

        if (string.IsNullOrEmpty(ssoTicket))
        {
            return;
        }

        if (GameClientManager.TryReconnection(ref Session, ssoTicket))
        {
            Session.SendPacket(new AuthenticationOKComposer());
            return;
        }

        if (Session.IsDisconnected)
        {
            Session.Disconnect();
            return;
        }

        try
        {
            using var dbClient = DatabaseManager.Connection;

            var ip = Session.Connection.Ip;
            var user = UserFactory.GetUserData(dbClient, ssoTicket, ip);

            if (user == null)
            {
                Session.Disconnect();
                return;
            }

            var packetList = new ServerPacketList();

            GameClientManager.LogClonesOut(user.Id);
            Session.User = user;
            Session.Language = user.Langue;
            Session.SSOTicket = ssoTicket;

            GameClientManager.RegisterClient(Session, user.Id, user.Username, ssoTicket);

            Session.User.Initialize(dbClient, Session);

            IsFirstConnexionToday(Session, dbClient, packetList);

            Session.SendPacket(new AuthenticationOKComposer());

            packetList.Add(new EconomyCenterComposer(EconomyCenterManager.EconomyCategory, EconomyCenterManager.EconomyItem));
            packetList.Add(new NavigatorHomeRoomComposer(Session.User.HomeRoom, Session.User.HomeRoom));
            packetList.Add(new FavouritesComposer(Session.User.FavoriteRooms));
            packetList.Add(new FigureSetIdsComposer());
            packetList.Add(new UserRightsComposer(Session.User.Rank < 2 ? 2 : Session.User.Rank, Session.User.Rank > 1));
            packetList.Add(new AvailabilityStatusComposer());
            packetList.Add(new AchievementScoreComposer(Session.User.AchievementPoints));
            packetList.Add(new BuildersClubMembershipComposer());
            packetList.Add(new CfhTopicsInitComposer(ModerationManager.UserActionPresets));
            packetList.Add(new UserSettingsComposer(Session.User.ClientVolume, Session.User.OldChat, Session.User.IgnoreRoomInvites, Session.User.CameraFollowDisabled, 1, 0));
            //packetList.Add(new AvatarEffectsComposer(EffectManager.Effects));

            packetList.Add(new CreditBalanceComposer(Session.User.Credits));

            // if (user.Rank > 12)
            // {
            //     var day = DateTime.Now.Day;
            //     var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            //     var missDays = new List<int>();
            //     for (var i = 0; i < day; i++)
            //     {
            //         missDays.Add(i);
            //     }

            //     packetList.Add(new CampaignCalendarDataComposer("premium", "/album1584/LOL.gif", day, days, missDays, missDays));
            //     packetList.Add(new InClientLinkComposer("openView/calendar"));
            // }

            if (IsNewUser(Session, dbClient))
            {
                packetList.Add(new NuxAlertComposer(2));
                packetList.Add(new InClientLinkComposer("nux/lobbyoffer/hide"));
            }

            if (Session.User.HasPermission("mod"))
            {
                GameClientManager.AddUserStaff(Session.User.Id);
                packetList.Add(new ModeratorInitComposer(
                    ModerationManager.UserMessagePresets,
                    ModerationManager.RoomMessagePresets,
                    ModerationManager.Tickets));
            }

            if (Session.User.HasPermission("helptool") && Session.User.BadgeComponent.HasBadgeSlot("STAFF_HELPER"))
            {
                HelpManager.TryAddGuide(Session.User.Id);
                Session.User.OnDuty = true;

                packetList.Add(new HelperToolComposer(Session.User.OnDuty, HelpManager.Count));
            }

            Session.SendPacket(packetList);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException("Invalid Dario bug duing user login: " + ex.ToString());
        }
    }

    private static void IsFirstConnexionToday(GameClient Session, IDbConnection dbClient, ServerPacketList packetList)
    {
        if (!Session.User.IsFirstConnexionToday)
        {
            return;
        }

        Session.User.IsFirstConnexionToday = false;

        var notifImage = "";
        var nbLot = 0;
        var respectCount = 5;
        var creditCount = 10000;
        var wibboPointCount = 0;
        var winwinCount = 0;

        if (Session.User.HasPermission("premium_legend"))
        {
            notifImage = "premium_legend";
            nbLot = 5;
            respectCount = 30;
            creditCount += creditCount * 3;
            wibboPointCount = 65;
            winwinCount = 100;
        }
        else if (Session.User.HasPermission("premium_epic"))
        {
            notifImage = "premium_epic";
            nbLot = 3;
            respectCount = 20;
            creditCount += creditCount * 2;
            wibboPointCount = 32;
            winwinCount = 50;
        }
        else if (Session.User.HasPermission("premium_classic"))
        {
            notifImage = "premium_classic";
            nbLot = 1;
            respectCount = 10;
            creditCount += creditCount;
            wibboPointCount = 12;
            winwinCount = 20;
        }

        if (nbLot > 0)
        {
            var lootboxId = SettingsManager.GetData<int>("givelot.lootbox.id");

            if (ItemManager.GetItem(lootboxId, out var itemData))
            {
                var items = ItemFactory.CreateMultipleItems(dbClient, itemData, Session.User, "", nbLot);

                foreach (var purchasedItem in items)
                {
                    Session.User.InventoryComponent.TryAddItem(purchasedItem);
                }
            }
        }

        if (wibboPointCount > 0)
        {
            Session.User.WibboPoints += wibboPointCount;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.WibboPoints, 0, 105));

            UserDao.UpdateAddPoints(dbClient, Session.User.Id, wibboPointCount);
        }

        if (winwinCount > 0)
        {
            UserStatsDao.UpdateAchievementScore(dbClient, Session.User.Id, winwinCount);

            Session.User.AchievementPoints += winwinCount;
        }

        if (Session.User.Credits <= int.MaxValue - creditCount)
        {
            Session.User.Credits += creditCount;
        }

        Session.User.DailyRespectPoints = respectCount;
        Session.User.DailyPetRespectPoints = respectCount;

        if (winwinCount > 0 || wibboPointCount > 0 || nbLot > 0)
        {
            packetList.Add(RoomNotificationComposer.SendBubble(notifImage, $"Vous avez reçu {wibboPointCount} WibboPoints, {winwinCount} Win-wins ainsi que {nbLot} LootBox!"));
        }
    }

    private static bool IsNewUser(GameClient Session, IDbConnection dbClient)
    {
        if (!Session.User.NewUser)
        {
            return false;
        }

        Session.User.NewUser = false;

        var homeId = SettingsManager.GetData<int>("default.home.id");

        var roomId = RoomDao.InsertDuplicate(dbClient, Session.User.Username, LanguageManager.TryGetValue("room.welcome.desc", Session.Language));

        UserDao.UpdateNuxEnable(dbClient, Session.User.Id, homeId > 0 ? homeId : roomId);

        Session.User.HomeRoom = homeId > 0 ? homeId : roomId;

        if (roomId == 0)
        {
            return false;
        }

        ItemDao.InsertDuplicate(dbClient, Session.User.Id, roomId);

        if (!Session.User.UsersRooms.Contains(roomId))
        {
            Session.User.UsersRooms.Add(roomId);
        }

        return true;
    }
}
