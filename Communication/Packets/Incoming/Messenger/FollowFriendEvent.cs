namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;

internal class FollowFriendEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User == null || !clientByUserId.User.InRoom || (clientByUserId.User.HideInRoom && !session.User.HasPermission("perm_mod")))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(clientByUserId.User.CurrentRoomId, out var room))
        {
            return;
        }

        session.SendPacket(new RoomForwardComposer(room.Id));
    }
}