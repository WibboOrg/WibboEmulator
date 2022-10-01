using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RemoveGroupFavouriteEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetUser().FavouriteGroupId = 0;

            if (Session.GetUser().InRoom)
            {
                RoomUser User = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                if (User != null)
                {
                    Session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, User.VirtualId));
                }

                Session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
            }
            else
            {
                Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
            }
        }
    }
}