using Wibbo.Communication.Packets.Outgoing.Users;
using Wibbo.Game.Clients;
using Wibbo.Game.Groups;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetUserGroupBadgesEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || Session.GetUser().LoadingRoomId == 0)
            {
                return;
            }

            Room Room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().LoadingRoomId);
            if (Room == null)
            {
                return;
            }

            Dictionary<int, string> Badges = new Dictionary<int, string>();
            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (User.IsBot || User.GetClient() == null || User.GetClient().GetUser() == null)
                {
                    continue;
                }

                if (User.GetClient().GetUser().FavouriteGroupId == 0 || Badges.ContainsKey(User.GetClient().GetUser().FavouriteGroupId))
                {
                    continue;
                }

                if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(User.GetClient().GetUser().FavouriteGroupId, out Group Group))
                {
                    continue;
                }

                if (!Badges.ContainsKey(Group.Id))
                {
                    Badges.Add(Group.Id, Group.Badge);
                }
            }

            if (Session.GetUser().FavouriteGroupId > 0)
            {
                if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(Session.GetUser().FavouriteGroupId, out Group Group))
                {
                    if (!Badges.ContainsKey(Group.Id))
                    {
                        Badges.Add(Group.Id, Group.Badge);
                    }
                }
            }

            Room.SendPacket(new UserGroupBadgesComposer(Badges));
            Session.SendPacket(new UserGroupBadgesComposer(Badges));
        }
    }
}
