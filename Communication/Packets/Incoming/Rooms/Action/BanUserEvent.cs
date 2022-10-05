namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class BanUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if ((room.RoomData.BanFuse != 1 || !room.CheckRights(session)) && !room.CheckRights(session, true))
        {
            return;
        }

        var pId = Packet.PopInt();
        var num = Packet.PopInt();
        var str = Packet.PopString();

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(pId);
        int Time;
        if (str.Equals("RWUAM_BAN_USER_HOUR"))
        {
            Time = 3600;
        }
        else if (str.Equals("RWUAM_BAN_USER_DAY"))
        {
            Time = 86400;
        }
        else
        {
            if (!str.Equals("RWUAM_BAN_USER_PERM"))
            {
                return;
            }

            Time = 429496729;
        }
        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.GetClient(), true) || roomUserByUserId.GetClient().GetUser().HasPermission("perm_kick"))
        {
            return;
        }

        room.AddBan(pId, Time);
        room.GetRoomUserManager().RemoveUserFromRoom(roomUserByUserId.GetClient(), true, true);
    }
}