namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class FollowFriendEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var clientByUserId = GameClientManager.GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User == null || !clientByUserId.User.InRoom || (clientByUserId.User.HideInRoom && !Session.User.HasPermission("mod")))
        {
            return;
        }

        if (!RoomManager.TryGetRoom(clientByUserId.User.RoomId, out var room))
        {
            return;
        }

        Session.SendPacket(new RoomForwardComposer(room.Id));
    }
}
