using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetHabboGroupBadgesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().LoadingRoomId == 0)
            {
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoomId);
            if (Room == null)
            {
                return;
            }

            Dictionary<int, string> Badges = new Dictionary<int, string>();
            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                {
                    continue;
                }

                if (User.GetClient().GetHabbo().FavouriteGroupId == 0 || Badges.ContainsKey(User.GetClient().GetHabbo().FavouriteGroupId))
                {
                    continue;
                }

                if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(User.GetClient().GetHabbo().FavouriteGroupId, out Guild Group))
                {
                    continue;
                }

                if (!Badges.ContainsKey(Group.Id))
                {
                    Badges.Add(Group.Id, Group.Badge);
                }
            }

            if (Session.GetHabbo().FavouriteGroupId > 0)
            {
                if (ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Session.GetHabbo().FavouriteGroupId, out Guild Group))
                {
                    if (!Badges.ContainsKey(Group.Id))
                    {
                        Badges.Add(Group.Id, Group.Badge);
                    }
                }
            }

            Room.SendPacket(new HabboGroupBadgesComposer(Badges));
            Session.SendPacket(new HabboGroupBadgesComposer(Badges));
        }
    }
}
