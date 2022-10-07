namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Games.GameClients;

internal class RpBotChooseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var message = packet.PopString();

        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null)
        {
            return;
        }

        var user = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (user == null)
        {
            return;
        }

        _ = room.AllowsShous(user, message);
    }
}
