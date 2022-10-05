namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class ApplySignEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        roomUserByUserId.Unidle();

        var num = Packet.PopInt();
        if (roomUserByUserId.ContainStatus("sign"))
        {
            roomUserByUserId.RemoveStatus("sign");
        }

        roomUserByUserId.SetStatus("sign", Convert.ToString(num));
        roomUserByUserId.UpdateNeeded = true;
    }
}
