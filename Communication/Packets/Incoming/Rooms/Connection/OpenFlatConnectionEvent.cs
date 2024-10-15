namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class OpenFlatConnectionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        var roomId = packet.PopInt();
        var password = packet.PopString();

        if (Session.User.InRoom)
        {
            if (RoomManager.TryGetRoom(Session.User.RoomId, out var oldRoom))
            {
                oldRoom.RoomUserManager.RemoveUserFromRoom(Session, false, false);
            }
        }

        Session.User.TryRemoveFromDoorBellList();

        if (Session.User.IsTeleporting && Session.User.TeleportingRoomID != roomId)
        {
            Session.User.TeleportingRoomID = 0;
            Session.User.IsTeleporting = false;
            Session.User.TeleporterId = 0;
            Session.SendPacket(new CloseConnectionComposer());

            return;
        }

        if (!RoomManager.TryGetRoom(roomId, out _))
        {
            if (Session.User.LastLoadedRoomTime.CheckIsBlocked())
            {
                Session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        var room = RoomManager.LoadRoom(roomId);
        if (room == null)
        {
            Session.SendPacket(new CloseConnectionComposer());
            return;
        }

        if (!Session.User.HasPermission("mod") && room.UserIsBanned(Session.User.Id))
        {
            if (room.HasBanExpired(Session.User.Id))
            {
                using (var dbClient = DatabaseManager.Connection)
                {
                    RoomBanDao.Delete(dbClient, room.Id, Session.User.Id);
                }

                room.RemoveBan(Session.User.Id);
            }
            else
            {
                Session.SendPacket(new CantConnectComposer(1));

                Session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !Session.User.HasPermission("enter_full_rooms") && !Session.User.HasPermission("enter_full_rooms"))
        {
            if (room.CloseFullRoom)
            {
                room.RoomData.Access = RoomAccess.Doorbell;
                room.CloseFullRoom = false;
            }

            if (Session.User.Id != room.RoomData.OwnerId)
            {
                Session.SendPacket(new CantConnectComposer(1));

                Session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        var ownerEnterNotAllowed = SettingsManager.GetData<string>("room.owner.enter.not.allowed").Split(',');

        if (!Session.User.HasPermission("access_apartments_all"))
        {
            if (!(Session.User.HasPermission("access_apartments") && !ownerEnterNotAllowed.Contains(room.RoomData.OwnerName)) && !room.CheckRights(Session, true) && !(Session.User.IsTeleporting && Session.User.TeleportingRoomID == room.Id))
            {
                if (room.RoomData.Access == RoomAccess.Doorbell && !room.CheckRights(Session))
                {
                    if (room.UserCount == 0)
                    {
                        Session.SendPacket(new FlatAccessDeniedComposer(""));
                    }
                    else
                    {
                        Session.SendPacket(new DoorbellComposer(""));
                        room.SendPacket(new DoorbellComposer(Session.User.Username), true);
                        Session.User.LoadingRoomId = roomId;
                        Session.User.IsRingingDoorBell = true;
                        Session.User.AllowDoorBell = false;
                    }
                    return;
                }
                else if (room.RoomData.Access == RoomAccess.Password && !password.Equals(room.RoomData.Password.ToLower(), StringComparison.CurrentCultureIgnoreCase))
                {
                    Session.SendPacket(new GenericErrorComposer(-100002));
                    Session.SendPacket(new CloseConnectionComposer());
                    return;
                }
            }
        }

        if (room.RoomData.OwnerName == SettingsManager.GetData<string>("autogame.owner") || room.CloseFullRoom)
        {
            if (room.RoomUserManager.GetUserByTracker(Session.User.IP) != null)
            {
                Session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        if (!Session.User.EnterRoom(room))
        {
            Session.SendPacket(new CloseConnectionComposer());
        }
        else
        {
            Session.User.LoadingRoomId = roomId;
            Session.User.AllowDoorBell = true;
        }
    }
}
