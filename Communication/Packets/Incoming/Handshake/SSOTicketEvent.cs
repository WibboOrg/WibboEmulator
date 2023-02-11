namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.BuildersClub;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Settings;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Authentificator;
using WibboEmulator.Utilities;

internal sealed class SSOTicketEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {        if (session == null || session.User != null)
        {
            return;
        }

        var ssoTicket = packet.PopString();
        _ = packet.PopInt();

        if (string.IsNullOrEmpty(ssoTicket))
        {
            return;
        }

        if (WibboEnvironment.GetGame().GetGameClientManager().TryReconnection(session, ssoTicket))
        {
            return;
        }

        try
        {
            var ip = session.Connection.GetIp();
            var user = UserFactory.GetUserData(ssoTicket, ip, session.MachineId);

            if (user == null)
            {
                return;
            }
            else
            {
                WibboEnvironment.GetGame().GetGameClientManager().LogClonesOut(user.Id);
                session.User = user;
                session.Langue = user.Langue;
                session.SSOTicket = ssoTicket;

                WibboEnvironment.GetGame().GetGameClientManager().RegisterClient(session, user.Id, user.Username);

                if (session.Langue == Language.French)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersFr++;
                }
                else if (session.Langue == Language.English)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersEn++;
                }
                else if (session.Langue == Language.Portuguese)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersBr++;
                }

                if (session.User.MachineId != session.MachineId && session.MachineId != null)
                {
                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    UserDao.UpdateMachineId(dbClient, session.User.Id, session.MachineId);
                }

                session.User.Init(session);

                session.SendPacket(new AuthenticationOKComposer());

                var packetList = new ServerPacketList();
                packetList.Add(new NavigatorHomeRoomComposer(session.User.HomeRoom, session.User.HomeRoom));
                //packetList.Add(new FavouritesComposer(session.User.FavoriteRooms));
                packetList.Add(new FigureSetIdsComposer());
                packetList.Add(new UserRightsComposer(session.User.Rank < 2 ? 2 : session.User.Rank));
                packetList.Add(new AvailabilityStatusComposer());
                packetList.Add(new AchievementScoreComposer(session.User.AchievementPoints));
                packetList.Add(new BuildersClubMembershipComposer());
                packetList.Add(new CfhTopicsInitComposer(WibboEnvironment.GetGame().GetModerationManager().UserActionPresets));
                packetList.Add(new UserSettingsComposer(session.User.ClientVolume, session.User.OldChat, session.User.IgnoreRoomInvites, session.User.CameraFollowDisabled, 1, 0));
                //packetList.Add(new AvatarEffectsComposer(WibboEnvironment.GetGame().GetEffectManager().Effects));

                packetList.Add(new ActivityPointNotificationComposer(session.User.Duckets, 1));
                packetList.Add(new CreditBalanceComposer(session.User.Credits));

                /*int day = (int)DateTime.Now.Day;
                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                List<int> missDays = new List<int>();
                for (int i = 0; i < day; i++)
                    missDays.Add(i);

                packetList.Add(new CampaignCalendarDataComposer("", "", day, days, new List<int>(), missDays));
                packetList.Add(new InClientLinkComposer("openView/calendar"));*/

                if (IsNewUser(session))
                {
                    packetList.Add(new NuxAlertComposer(2));
                    packetList.Add(new InClientLinkComposer("nux/lobbyoffer/hide"));
                }

                if (session.User.HasPermission("mod"))
                {
                    WibboEnvironment.GetGame().GetGameClientManager().AddUserStaff(session.User.Id);
                    packetList.Add(new ModeratorInitComposer(
                        WibboEnvironment.GetGame().GetModerationManager().UserMessagePresets(),
                        WibboEnvironment.GetGame().GetModerationManager().RoomMessagePresets(),
                        WibboEnvironment.GetGame().GetModerationManager().Tickets()));
                }

                if (session.User.HasExactPermission("helptool"))
                {
                    var guideManager = WibboEnvironment.GetGame().GetHelpManager();
                    guideManager.AddGuide(session.User.Id);
                    session.User.OnDuty = true;

                    packetList.Add(new HelperToolComposer(session.User.OnDuty, guideManager.GuidesCount));
                }

                session.SendPacket(packetList);

                return;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException("Invalid Dario bug duing user login: " + ex.ToString());
        }
    }

    private static bool IsNewUser(GameClient session)
    {
        if (!session.User.NewUser)
        {
            return false;
        }

        session.User.NewUser = false;

        var roomId = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            roomId = RoomDao.InsertDuplicate(dbClient, session.User.Username, WibboEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", session.Langue));

            UserDao.UpdateNuxEnable(dbClient, session.User.Id, roomId);
            if (roomId == 0)
            {
                return false;
            }

            ItemDao.InsertDuplicate(dbClient, session.User.Id, roomId);
        }

        if (!session.User.UsersRooms.Contains(roomId))
        {
            session.User.UsersRooms.Add(roomId);
        }

        session.User.HomeRoom = roomId;

        return true;
    }
}
