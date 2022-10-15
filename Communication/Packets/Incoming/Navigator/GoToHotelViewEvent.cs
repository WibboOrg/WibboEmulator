namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;

internal class GoToHotelViewEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        session.SendPacket(new CloseConnectionComposer());
        session.User.LoadingRoomId = 0;

        if (session.User == null || !session.User.InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        room.
        RoomUserManager.RemoveUserFromRoom(session, false, false);
    }
}