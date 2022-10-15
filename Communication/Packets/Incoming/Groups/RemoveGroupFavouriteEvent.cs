namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;

internal class RemoveGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        session.User.FavouriteGroupId = 0;

        if (session.User.InRoom)
        {
            var userRoom = session.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(session.User.Id);
            if (userRoom != null)
            {
                session.User.CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, userRoom.VirtualId));
            }

            session.
            User.CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
        }
    }
}
