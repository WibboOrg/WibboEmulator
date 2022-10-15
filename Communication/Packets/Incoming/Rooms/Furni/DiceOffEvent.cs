namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;

internal class DiceOffEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(packet.PopInt());
        if (roomItem == null)
        {
            return;
        }

        var userHasRights = false;
        if (room.CheckRights(session))
        {
            userHasRights = true;
        }

        roomItem.Interactor.OnTrigger(session, roomItem, -1, userHasRights, false);
        roomItem.OnTrigger(room.RoomUserManager.GetRoomUserByUserId(session.User.Id));
    }
}
