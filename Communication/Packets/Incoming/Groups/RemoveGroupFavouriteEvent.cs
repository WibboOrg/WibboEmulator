namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;

internal sealed class RemoveGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        session.User.FavouriteGroupId = 0;

        var room = session.User.CurrentRoom;
        if (room != null)
        {
            var userRoom = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
            if (userRoom != null)
            {
                room.SendPacket(new UpdateFavouriteGroupComposer(null, userRoom.VirtualId));
            }

            room.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
        }
    }
}
