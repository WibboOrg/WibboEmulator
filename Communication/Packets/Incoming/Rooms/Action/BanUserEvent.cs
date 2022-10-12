namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class BanUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if ((room.Data.BanFuse != 1 || !room.CheckRights(session)) && !room.CheckRights(session, true))
        {
            return;
        }

        var pId = packet.PopInt();

        _ = packet.PopInt();
        var str = packet.PopString();

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(pId);
        int time;
        if (str.Equals("RWUAM_BAN_USER_HOUR"))
        {
            time = 3600;
        }
        else if (str.Equals("RWUAM_BAN_USER_DAY"))
        {
            time = 86400;
        }
        else
        {
            if (!str.Equals("RWUAM_BAN_USER_PERM"))
            {
                return;
            }

            time = 429496729;
        }
        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.Client, true) || roomUserByUserId.Client.GetUser().HasPermission("perm_kick"))
        {
            return;
        }

        room.AddBan(pId, time);
        room.GetRoomUserManager().RemoveUserFromRoom(roomUserByUserId.Client, true, true);
    }
}
