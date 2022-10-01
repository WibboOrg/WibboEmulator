using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetUserGroupBadgesEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || Session.GetUser().LoadingRoomId == 0)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().LoadingRoomId, out Room room))
                return;

            Dictionary<int, string> Badges = new Dictionary<int, string>();
            foreach (RoomUser User in room.GetRoomUserManager().GetRoomUsers().ToList())
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

            room.SendPacket(new UserGroupBadgesComposer(Badges));
            Session.SendPacket(new UserGroupBadgesComposer(Badges));
        }
    }
}
