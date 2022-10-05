namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RpBotChooseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var Message = Packet.PopString();

        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var Room = session.GetUser().CurrentRoom;
        if (Room == null)
        {
            return;
        }

        var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (User == null)
        {
            return;
        }

        Room.AllowsShous(User, Message);
    }
}