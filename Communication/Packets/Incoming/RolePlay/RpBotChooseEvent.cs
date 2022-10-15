namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Games.GameClients;

internal class RpBotChooseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var message = packet.PopString();

        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        _ = room.AllowsShous(user, message);
    }
}
