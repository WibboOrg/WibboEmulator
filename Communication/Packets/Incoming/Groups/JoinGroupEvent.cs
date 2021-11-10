using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Game.GameClients;
using Butterfly.Game.Groups;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class JoinGroupEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group Group))
            {
                return;
            }

            if (Group.IsMember(Session.GetHabbo().Id) || Group.IsAdmin(Session.GetHabbo().Id) || (Group.HasRequest(Session.GetHabbo().Id) && Group.GroupType == GroupType.LOCKED) || Group.GroupType == GroupType.PRIVATE)
            {
                return;
            }

            if (Session.GetHabbo().MyGroups.Count >= 50)
            {
                Session.SendNotification("Oups, il semble que vous avez atteint la limite d'adhésion au groupe! Vous pouvez seulement rejoindre jusqu'à 50 groupes.");
                return;
            }

            Group.AddMember(Session.GetHabbo().Id);

            if (Group.GroupType == GroupType.LOCKED)
            {
                /*List<GameClient> GroupAdmins = (from Client in ButterflyEnvironment.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null && Group.IsAdmin(Client.GetHabbo().Id) select Client).ToList();
                foreach (GameClient Client in GroupAdmins)
                {
                    Client.SendPacket(new GroupMembershipRequestedComposer(Group.Id, Session.GetHabbo(), 3));
                }*/

                Session.SendPacket(new GroupInfoComposer(Group, Session));
            }
            else
            {
                Session.GetHabbo().MyGroups.Add(Group.Id);
                Session.SendPacket(new GroupFurniConfigComposer(ButterflyEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().MyGroups)));
                Session.SendPacket(new GroupInfoComposer(Group, Session));

                if (Session.GetHabbo().CurrentRoom != null)
                {
                    Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                }
                else
                {
                    Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                }
            }

        }
    }
}
