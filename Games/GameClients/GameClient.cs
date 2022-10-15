namespace WibboEmulator.Games.GameClients;
using WibboEmulator.Communication.Interfaces;
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
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Settings;
using WibboEmulator.Communication.Packets.Outgoing.WibboTool;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Users.Authentificator;
using WibboEmulator.Utilities;

public class GameClient
{
    private GameWebSocket _connection;
    private User _user;

    private Dictionary<int, double> _packetTimeout;
    private int _packetCount;
    private double _packetLastTimestamp;

    public string MachineId { get; set; }
    public Language Langue { get; set; }

    public string ConnectionID { get; set; }
    public bool ShowGameAlert { get; set; }

    public GameClient(string clientId, GameWebSocket connection)
    {
        this.ConnectionID = clientId;
        this.Langue = Language.French;
        this._connection = connection;

        this._packetTimeout = new Dictionary<int, double>();
        this._packetCount = 0;
        this._packetLastTimestamp = UnixTimestamp.GetNow();
    }

    public void TryAuthenticate(string authTicket)
    {
        if (string.IsNullOrEmpty(authTicket))
        {
            return;
        }

        try
        {
            var ip = this.GetConnection().GetIp();
            var user = UserFactory.GetUserData(authTicket, ip, this.MachineId);

            if (user == null)
            {
                return;
            }
            else
            {
                WibboEnvironment.GetGame().GetGameClientManager().LogClonesOut(user.Id);
                this._user = user;
                this.Langue = user.Langue;

                WibboEnvironment.GetGame().GetGameClientManager().RegisterClient(this, user.Id, user.Username);

                if (this.Langue == Language.French)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersFr++;
                }
                else if (this.Langue == Language.English)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersEn++;
                }
                else if (this.Langue == Language.Portuguese)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersBr++;
                }

                if (this._user.MachineId != this.MachineId && this.MachineId != null)
                {
                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    UserDao.UpdateMachineId(dbClient, this._user.Id, this.MachineId);
                }

                this._user.Init(this);

                this.SendPacket(new AuthenticationOKComposer());

                var packetList = new ServerPacketList();
                packetList.Add(new NavigatorHomeRoomComposer(this._user.HomeRoom, this._user.HomeRoom));
                packetList.Add(new FavouritesComposer(this._user.FavoriteRooms));
                packetList.Add(new FigureSetIdsComposer());
                packetList.Add(new UserRightsComposer(this._user.Rank < 2 ? 2 : this.GetUser().Rank));
                packetList.Add(new AvailabilityStatusComposer());
                packetList.Add(new AchievementScoreComposer(this._user.AchievementPoints));
                packetList.Add(new BuildersClubMembershipComposer());
                packetList.Add(new CfhTopicsInitComposer(WibboEnvironment.GetGame().GetModerationManager().UserActionPresets));
                packetList.Add(new UserSettingsComposer(this._user.ClientVolume, this._user.OldChat, this._user.IgnoreRoomInvites, this._user.CameraFollowDisabled, 1, 0));
                packetList.Add(new AvatarEffectsComposer(WibboEnvironment.GetGame().GetEffectManager().GetEffects()));

                packetList.Add(new ActivityPointNotificationComposer(this._user.Duckets, 1));
                packetList.Add(new CreditBalanceComposer(this._user.Credits));

                /*int day = (int)DateTime.Now.Day;
                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                List<int> missDays = new List<int>();
                for (int i = 0; i < day; i++)
                    missDays.Add(i);

                packetList.Add(new CampaignCalendarDataComposer("", "", day, days, new List<int>(), missDays));
                packetList.Add(new InClientLinkComposer("openView/calendar"));*/

                if (this.IsNewUser())
                {
                    packetList.Add(new NuxAlertComposer(2));
                    packetList.Add(new InClientLinkComposer("nux/lobbyoffer/hide"));
                }

                if (this._user.HasPermission("perm_mod"))
                {
                    WibboEnvironment.GetGame().GetGameClientManager().AddUserStaff(this._user.Id);
                    packetList.Add(new ModeratorInitComposer(
                        WibboEnvironment.GetGame().GetModerationManager().UserMessagePresets(),
                        WibboEnvironment.GetGame().GetModerationManager().RoomMessagePresets(),
                        WibboEnvironment.GetGame().GetModerationManager().Tickets()));
                }

                if (this._user.HasExactPermission("perm_helptool"))
                {
                    var guideManager = WibboEnvironment.GetGame().GetHelpManager();
                    guideManager.AddGuide(this._user.Id);
                    this._user.OnDuty = true;

                    packetList.Add(new HelperToolComposer(this._user.OnDuty, guideManager.GuidesCount));
                }

                this.SendPacket(packetList);

                return;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException("Invalid Dario bug duing user login: " + ex.ToString());
        }
    }

    private bool IsNewUser()
    {
        if (!this.GetUser().NewUser)
        {
            return false;
        }

        this.GetUser().NewUser = false;

        var roomId = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            roomId = RoomDao.InsertDuplicate(dbClient, this.GetUser().Username, WibboEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));

            UserDao.UpdateNuxEnable(dbClient, this.GetUser().Id, roomId);
            if (roomId == 0)
            {
                return false;
            }

            ItemDao.InsertDuplicate(dbClient, this.GetUser().Id, roomId);
        }

        if (!this.GetUser().UsersRooms.Contains(roomId))
        {
            this.GetUser().UsersRooms.Add(roomId);
        }

        this.GetUser().HomeRoom = roomId;

        return true;
    }

    public GameWebSocket GetConnection() => this._connection;

    public User GetUser() => this._user;

    public bool Antipub(string message, string type, int roomId = 0)
    {
        if (this.GetUser() == null)
        {
            return false;
        }

        if (this.GetUser().HasPermission("perm_god"))
        {
            return false;
        }

        if (message.Length <= 3)
        {
            return false;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            LogChatDao.Insert(dbClient, this.GetUser().Id, roomId, message, type, this.GetUser().Username);
        }

        if (!WibboEnvironment.GetGame().GetChatManager().GetFilter().Ispub(message))
        {
            if (WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(message))
            {
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    LogChatPubDao.Insert(dbClient, this.GetUser().Id, "A vérifié: " + type + message, this.GetUser().Username);
                }

                foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers())
                {
                    if (client == null || client.GetUser() == null)
                    {
                        continue;
                    }

                    client.SendPacket(new AddChatlogsComposer(this._user.Id, this._user.Username, type + message));
                }

                return false;
            }

            return false;
        }

        var pubCount = this.GetUser().PubDetectCount++;

        if (type == "<CMD>")
        {
            pubCount = 4;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            LogChatPubDao.Insert(dbClient, this.GetUser().Id, "Pub numero " + pubCount + ": " + type + message, this.GetUser().Username);
        }

        if (pubCount is < 3 and > 0)
        {
            this.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), pubCount));
        }
        else if (pubCount == 3)
        {
            this.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
        }
        else if (pubCount == 4)
        {
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(this, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.GetUser().Username, true, false);
        }

        foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers())
        {
            if (client == null || client.GetUser() == null)
            {
                continue;
            }

            client.SendPacket(RoomNotificationComposer.SendBubble("mention", "Detection d'un message suspect sur le compte " + this.GetUser().Username));
            client.SendPacket(new AddChatlogsComposer(this._user.Id, this._user.Username, type + message));
        }
        return true;
    }
    public void SendWhisper(string message, bool info = true)
    {
        if (this.GetUser() == null || this.GetUser().CurrentRoom == null)
        {
            return;
        }

        var user = this.GetUser().CurrentRoom.RoomUserManager.GetRoomUserByName(this.GetUser().Username);
        if (user == null)
        {
            return;
        }

        this.SendPacket(new WhisperComposer(user.VirtualId, message, info ? 34 : 0));
    }
    public void SendNotification(string message) => this.SendPacket(new BroadcastMessageAlertComposer(message));

    public void SendHugeNotif(string message) => this.SendPacket(new MOTDNotificationComposer(message));

    public bool PacketTimeout(int packetId, double delay)
    {
        var timestampNow = UnixTimestamp.GetNow();

        if (this._packetLastTimestamp > timestampNow)
        {
            this._packetCount++;
        }
        else
        {
            this._packetCount = 0;
            this._packetLastTimestamp = timestampNow + 1;
        }

        if (this._packetCount >= 20)
        {
            return true;
        }

        if (delay <= 0)
        {
            return false;
        }

        if (this._packetTimeout.TryGetValue(packetId, out var timestamp))
        {
            if (timestamp > timestampNow)
            {
                return true;
            }

            _ = this._packetTimeout.Remove(packetId);
        }

        this._packetTimeout.Add(packetId, timestampNow + (delay / 1000));

        return false;
    }

    public void Dispose()
    {
        if (this.Langue == Language.French)
        {
            WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersFr--;
        }
        else if (this.Langue == Language.English)
        {
            WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersEn--;
        }
        else if (this.Langue == Language.Portuguese)
        {
            WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersBr--;
        }

        if (this.GetUser() != null)
        {
            this._user.Dispose();
        }

        if (this._packetTimeout != null)
        {
            this._packetTimeout.Clear();
        }

        this._packetTimeout = null;
        this._user = null;
        this._connection = null;
    }

    public void Disconnect()
    {
        if (this._connection != null)
        {
            this._connection.Disconnect();
        }
    }

    public void SendPacket(ServerPacketList packets)
    {
        if (packets == null)
        {
            return;
        }

        if (packets.Count == 0)
        {
            return;
        }

        this.GetConnection().SendData(packets.GetBytes);
    }

    public void SendPacket(IServerPacket packet)
    {
        if (packet == null || this.GetConnection() == null)
        {
            return;
        }

        this.GetConnection().SendData(packet.GetBytes());
    }
}
