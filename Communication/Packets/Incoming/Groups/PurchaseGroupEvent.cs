namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class PurchaseGroupEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var Name = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
        var Description = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
        var RoomId = packet.PopInt();
        var Colour1 = packet.PopInt();
        var Colour2 = packet.PopInt();
        var Unknown = packet.PopInt();

        if (Name.Length > 50)
        {
            return;
        }

        if (Description.Length > 255)
        {
            return;
        }

        if (Colour1 is < 0 or > 200)
        {
            return;
        }

        if (Colour2 is < 0 or > 200)
        {
            return;
        }

        var Room = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
        if (Room == null || Room.OwnerId != session.GetUser().Id || Room.Group != null)
        {
            return;
        }

        var Badge = string.Empty;

        for (var i = 0; i < 5; i++)
        {
            Badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryCreateGroup(session.GetUser(), Name, Description, RoomId, Badge, Colour1, Colour2, out var Group))
        {
            return;
        }

        session.SendPacket(new PurchaseOKComposer());

        Room.Group = Group;

        var groupCost = 20;

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