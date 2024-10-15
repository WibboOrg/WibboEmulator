namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;

internal sealed class RemoveGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        Session.User.FavouriteGroupId = 0;

        var room = Session.User.Room;
        if (room != null)
        {
            var userRoom = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
            if (userRoom != null)
            {
                room.SendPacket(new UpdateFavouriteGroupComposer(null, userRoom.VirtualId));
            }

            room.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
        }
        else
        {
            Session.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
        }
    }
}
