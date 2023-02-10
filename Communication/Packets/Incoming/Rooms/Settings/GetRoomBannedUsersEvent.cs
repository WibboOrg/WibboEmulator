namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal sealed class GetRoomBannedUsersEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        var room = session.User.CurrentRoom;
        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        if (room.GetBans().Count > 0)
        {
            session.SendPacket(new GetRoomBannedUsersComposer(room));
        }
    }
}
