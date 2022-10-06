namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Games.GameClients;

internal class GoToHotelViewEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        session.SendPacket(new CloseConnectionComposer());
        session.GetUser().LoadingRoomId = 0;

        if (session.GetUser() == null || !session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        room.GetRoomUserManager().RemoveUserFromRoom(session, false, false);
    }
}