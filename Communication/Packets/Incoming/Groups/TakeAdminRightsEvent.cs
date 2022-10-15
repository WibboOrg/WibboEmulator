namespace WibboEmulator.Communication.Packets.Incoming.Groups;using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Games.GameClients;

internal class TakeAdminRightsEvent : IPacketEvent{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)    {        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (session.User.Id != group.CreatorId || !group.IsMember(userId))
        {
            return;
        }

        var user = WibboEnvironment.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        group.TakeAdmin(userId);

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out var room))
        {
            var userRoom = room.RoomUserManager.GetRoomUserByUserId(userId);
            if (userRoom != null)
            {
                if (userRoom.ContainStatus("flatctrl"))
                {
                    userRoom.RemoveStatus("flatctrl");
                }

                userRoom.UpdateNeeded = true;
                if (userRoom.Client != null)
                {
                    userRoom.Client.SendPacket(new YouAreControllerComposer(0));
                }
            }
        }

        session.SendPacket(new GroupMemberUpdatedComposer(groupId, user, 2));    }}