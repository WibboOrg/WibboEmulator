namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal sealed class GetRoomBannedUsersEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null || !room.CheckRights(Session, true))
        {
            return;
        }

        if (room.Bans.Count > 0)
        {
            Session.SendPacket(new GetRoomBannedUsersComposer(room));
        }
    }
}
