namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal sealed class GetRoomRightsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        var rooom = Session.User.Room;
        if (rooom == null)
        {
            return;
        }

        if (!rooom.CheckRights(Session))
        {
            return;
        }

        Session.SendPacket(new RoomRightsListComposer(rooom));
    }
}
