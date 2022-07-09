using Wibbo.Communication.Packets.Outgoing.Catalog;
using Wibbo.Communication.Packets.Outgoing.Groups;
using Wibbo.Communication.Packets.Outgoing.Inventory.Purse;
using Wibbo.Communication.Packets.Outgoing.Rooms.Session;
using Wibbo.Game.Clients;
using Wibbo.Game.Groups;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class PurchaseGroupEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client session, ClientPacket packet)
        {
            string Name = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
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

            RoomData Room = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Room == null || Room.OwnerId != session.GetUser().Id || Room.Group != null)
            {
                return;
            }

            string Badge = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
            }

            if (!WibboEnvironment.GetGame().GetGroupManager().TryCreateGroup(session.GetUser(), Name, Description, RoomId, Badge, Colour1, Colour2, out Group Group))
            {
                return;
            }

            session.SendPacket(new PurchaseOKComposer());

            Room.Group = Group;

            int groupCost = 20;

            if (session.GetUser().Credits < groupCost)
            {
                return;
            }

            session.GetUser().Credits -= groupCost;
            session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));

            if (session.GetUser().CurrentRoomId != Room.Id)
            {
                session.SendPacket(new RoomForwardComposer(Room.Id));
            }

            session.SendPacket(new NewGroupInfoComposer(RoomId, Group.Id));
        }
    }
}