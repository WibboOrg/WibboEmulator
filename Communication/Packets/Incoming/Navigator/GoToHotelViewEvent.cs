namespace WibboEmulator.Communication.Packets.Incoming.Navigator;

using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GoToHotelViewEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        Session.SendPacket(new CloseConnectionComposer());
        Session.User.TryRemoveFromDoorBellList();
        Session.User.LoadingRoomId = 0;

        if (Session.User == null || !Session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        room.RoomUserManager.RemoveUserFromRoom(Session, false, false);
    }
}
