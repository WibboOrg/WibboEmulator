namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class OpenFlatConnectionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var roomId = packet.PopInt();
        var password = packet.PopString();

        if (session.User.InRoom)
        {
            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var oldRoom))
            {
                oldRoom.RoomUserManager.RemoveUserFromRoom(session, false, false);
            }
        }

        if (session.User.IsTeleporting && session.User.TeleportingRoomID != roomId)
        {
            session.User.TeleportingRoomID = 0;
            session.User.IsTeleporting = false;
            session.User.TeleporterId = 0;
            session.SendPacket(new CloseConnectionComposer());

            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out _))
        {
            if (session.User.LastLoadedRoomTime.CheckIsBlocked())
            {
                session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        var room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);
        if (room == null)
        {
            session.SendPacket(new CloseConnectionComposer());
            return;
        }

        if (!session.User.HasPermission("perm_mod") && room.UserIsBanned(session.User.Id))
        {
            if (room.HasBanExpired(session.User.Id))
            {
                room.RemoveBan(session.User.Id);
            }
            else
            {
                session.SendPacket(new CantConnectComposer(1));

                session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !session.User.HasPermission("perm_enter_full_rooms") && !session.User.HasPermission("perm_enter_full_rooms"))
        {
            if (room.CloseFullRoom)
            {
                room.RoomData.Access = RoomAccess.Doorbell;
                room.CloseFullRoom = false;
            }

            if (session.User.Id != room.RoomData.OwnerId)
            {
                session.SendPacket(new CantConnectComposer(1));

                session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        var ownerEnterNotAllowed = WibboEnvironment.GetSettings().GetData<string>("room.owner.enter.not.allowed").Split(',');

        if (!session.User.HasPermission("perm_access_apartments_all"))
        {
            if (!(session.User.HasPermission("perm_access_apartments") && !ownerEnterNotAllowed.Contains(room.RoomData.OwnerName)) && !room.CheckRights(session, true) && !(session.User.IsTeleporting && session.User.TeleportingRoomID == room.Id))
            {
                if (room.RoomData.Access == RoomAccess.Doorbell && !room.CheckRights(session))
                {
                    if (room.UserCount == 0)
                    {
                        session.SendPacket(new FlatAccessDeniedComposer(""));
                    }
                    else
                    {
                        session.SendPacket(new DoorbellComposer(""));
                        room.SendPacket(new DoorbellComposer(session.User.Username), true);
                        session.User.LoadingRoomId = roomId;
                        session.User.AllowDoorBell = false;
                    }
                    return;
                }
                else if (room.RoomData.Access == RoomAccess.Password && password.ToLower() != room.RoomData.Password.ToLower())
                {
                    session.SendPacket(new GenericErrorComposer(-100002));
                    session.SendPacket(new CloseConnectionComposer());
                    return;
                }
            }
        }

        if (room.RoomData.OwnerName == WibboEnvironment.GetSettings().GetData<string>("autogame.owner"))
        {
            if (room.RoomUserManager.GetUserByTracker(session.User.IP, session.MachineId) != null)
            {
                session.SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        if (!session.User.EnterRoom(room))
        {
            session.SendPacket(new CloseConnectionComposer());
        }
        else
        {
            session.User.LoadingRoomId = roomId;
            session.User.AllowDoorBell = true;
        }
    }
}
