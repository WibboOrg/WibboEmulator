using Butterfly.Communication.Packets.Outgoing.Groups;

using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.GetHabbo().FavouriteGroupId = 0;

            if (Session.GetHabbo().InRoom)
            {
                RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (User != null)
                {
                    Session.GetHabbo().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, User.VirtualId));
                }

                Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }
            else
            {
                Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }
        }
    }
}