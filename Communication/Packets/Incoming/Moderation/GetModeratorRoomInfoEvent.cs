namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GetModeratorRoomInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("mod"))
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

        Session.SendPacket(new ModeratorRoomInfoComposer(data, ownerInRoom));
    }
}
