namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;

internal class ApplySignEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
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
