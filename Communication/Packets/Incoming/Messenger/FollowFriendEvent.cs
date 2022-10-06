namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Games.GameClients;

internal class FollowFriendEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.GetUser() == null || !clientByUserId.GetUser().InRoom || (clientByUserId.GetUser().HideInRoom && !session.GetUser().HasPermission("perm_mod")))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(clientByUserId.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        session.SendPacket(new RoomForwardComposer(room.Id));
    }
}