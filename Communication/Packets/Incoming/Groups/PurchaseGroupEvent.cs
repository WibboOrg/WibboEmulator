using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class PurchaseGroupEvent : IPacketEvent
    {
        public void Parse(Client session, ClientPacket packet)
        {
            string Name = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            int RoomId = packet.PopInt();
            int Colour1 = packet.PopInt();
            int Colour2 = packet.PopInt();
            int Unknown = packet.PopInt();

            if (Name.Length > 50)
            {
                return;
            }

            if (Description.Length > 255)
            {
                return;
            }

            if (Colour1 < 0 || Colour1 > 200)
            {
                return;
            }

            if (Colour2 < 0 || Colour2 > 200)
            {
                return;
            }

            RoomData Room = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Room == null || Room.OwnerId != session.GetHabbo().Id || Room.Group != null)
            {
                return;
            }

            string Badge = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
            }

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryCreateGroup(session.GetHabbo(), Name, Description, RoomId, Badge, Colour1, Colour2, out Group Group))
            {
                return;
            }

            session.SendPacket(new PurchaseOKComposer());

            Room.Group = Group;

            int groupCost = 20;

            if (session.GetHabbo().Credits < groupCost)
            {
                return;
            }

            session.GetHabbo().Credits -= groupCost;
            session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));

            if (session.GetHabbo().CurrentRoomId != Room.Id)
            {
                session.SendPacket(new RoomForwardComposer(Room.Id));
            }

            session.SendPacket(new NewGroupInfoComposer(RoomId, Group.Id));
        }
    }
}