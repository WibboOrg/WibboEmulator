namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal class GiveAdminRightsEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var GroupId = Packet.PopInt();
        var UserId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        if (session.GetUser().Id != Group.CreatorId || !Group.IsMember(UserId))
        {
            return;
        }

        var user = WibboEnvironment.GetUserById(UserId);
        if (user == null)
        {
            return;
        }

        Group.MakeAdmin(UserId);

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out var Room))
        {
            var User = Room.GetRoomUserManager().GetRoomUserByUserId(UserId);
            if (User != null)
            {
                if (!User.ContainStatus("flatctrl"))
                {
                    User.SetStatus("flatctrl", "1");
                }

                User.UpdateNeeded = true;

                if (User.GetClient() != null)
                {
                    User.GetClient().SendPacket(new YouAreControllerComposer(1));
                }
            }
        }

        session.SendPacket(new GroupMemberUpdatedComposer(GroupId, user, 1));
    }
}