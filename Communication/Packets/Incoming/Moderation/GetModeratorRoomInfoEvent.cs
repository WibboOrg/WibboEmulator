namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GetModeratorRoomInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("mod"))
        {
            return;
        }

        var roomId = packet.PopInt();

        var data = RoomManager.GenerateNullableRoomData(roomId);

        var ownerInRoom = false;
        if (RoomManager.TryGetRoom(data.Id, out var room))
        {
            if (room.RoomUserManager.GetRoomUserByName(data.OwnerName) != null)
            {
                ownerInRoom = true;
            }
        }

        session.SendPacket(new ModeratorRoomInfoComposer(data, ownerInRoom));
    }
}
