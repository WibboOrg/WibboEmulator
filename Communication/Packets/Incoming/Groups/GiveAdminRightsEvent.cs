namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Games.GameClients;

internal class GiveAdminRightsEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (session.GetUser().Id != group.CreatorId || !group.IsMember(userId))
        {
            return;
        }

        var user = WibboEnvironment.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        group.MakeAdmin(userId);

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out var room))
        {
            var userRooom = room.GetRoomUserManager().GetRoomUserByUserId(userId);
            if (userRooom != null)
            {
                if (!userRooom.ContainStatus("flatctrl"))
                {
                    userRooom.SetStatus("flatctrl", "1");
                }

                userRooom.UpdateNeeded = true;

                if (userRooom.GetClient() != null)
                {
                    userRooom.GetClient().SendPacket(new YouAreControllerComposer(1));
                }
            }
        }

        session.SendPacket(new GroupMemberUpdatedComposer(groupId, user, 1));
    }
}
