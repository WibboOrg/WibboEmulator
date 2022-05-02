using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Game.Clients;
using Butterfly.Game.Groups;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class JoinGroupEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group Group))
            {
                return;
            }

            if (Group.IsMember(Session.GetUser().Id) || Group.IsAdmin(Session.GetUser().Id) || (Group.HasRequest(Session.GetUser().Id) && Group.GroupType == GroupType.LOCKED) || Group.GroupType == GroupType.PRIVATE)
            {
                return;
            }

            if (Session.GetUser().MyGroups.Count >= 50)
            {
                Session.SendNotification("Oups, il semble que vous avez atteint la limite d'adhésion au groupe! Vous pouvez seulement rejoindre jusqu'à 50 groupes.");
                return;
            }

            Group.AddMember(Session.GetUser().Id);

            if (Group.GroupType == GroupType.LOCKED)
            {
                Session.SendPacket(new GroupInfoComposer(Group, Session));
            }
            else
            {
                Session.GetUser().MyGroups.Add(Group.Id);
                Session.SendPacket(new GroupFurniConfigComposer(ButterflyEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetUser().MyGroups)));
                Session.SendPacket(new GroupInfoComposer(Group, Session));

                if (Session.GetUser().CurrentRoom != null)
                {
                    Session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
                }
                else
                {
                    Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
                }
            }
        }
    }
}
