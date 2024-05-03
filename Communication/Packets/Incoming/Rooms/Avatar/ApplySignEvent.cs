namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ApplySignEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        roomUserByUserId.Unidle();

        var num = packet.PopInt();
        if (roomUserByUserId.ContainStatus("sign"))
        {
            roomUserByUserId.RemoveStatus("sign");
        }

        roomUserByUserId.SetStatus("sign", Convert.ToString(num));
        roomUserByUserId.UpdateNeeded = true;
    }
}
