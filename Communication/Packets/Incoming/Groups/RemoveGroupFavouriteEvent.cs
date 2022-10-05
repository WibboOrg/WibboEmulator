namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;

internal class RemoveGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        session.GetUser().FavouriteGroupId = 0;

        if (session.GetUser().InRoom)
        {
            var User = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (User != null)
            {
                session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, User.VirtualId));
            }

            session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
        }
    }
}