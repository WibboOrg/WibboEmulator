namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;

internal class GetModeratorRoomInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("mod"))
        {
            return;
        }

        var roomId = packet.PopInt();

        var data = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(roomId);

        var ownerInRoom = false;
        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(data.Id, out var room))
        {
            if (room.RoomUserManager.GetRoomUserByName(data.OwnerName) != null)
            {
                ownerInRoom = true;
            }
        }

        session.SendPacket(new ModeratorRoomInfoComposer(data, ownerInRoom));
    }
}
