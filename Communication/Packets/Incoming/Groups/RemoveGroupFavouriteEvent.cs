namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;

internal class RemoveGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        session.GetUser().FavouriteGroupId = 0;

        if (session.GetUser().InRoom)
        {
            var userRoom = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (userRoom != null)
            {
                session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, userRoom.VirtualId));
            }

            session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
        }
    }
}
