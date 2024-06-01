namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class PurchaseGroupEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var name = WordFilterManager.CheckMessage(packet.PopString(16));
        var description = WordFilterManager.CheckMessage(packet.PopString());
        var roomId = packet.PopInt();
        var colour1 = packet.PopInt();
        var colour2 = packet.PopInt();

        _ = packet.PopInt();

        var groupCost = 20;

        if (session.User.Credits < groupCost)
        {
            return;
        }

        if (name.Length > 50)
        {
            return;
        }

        if (description.Length > 255)
        {
            return;
        }

        if (colour1 is < 0 or > 200)
        {
            return;
        }

        if (colour2 is < 0 or > 200)
        {
            return;
        }

        var roomData = RoomManager.GenerateRoomData(roomId);
        if (roomData == null || roomData.OwnerId != session.User.Id || roomData.Group != null)
        {
            return;
        }

        var badge = string.Empty;

        for (var i = 0; i < 5; i++)
        {
            badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
        }

        if (!GroupManager.TryCreateGroup(session.User, name, description, roomId, badge, colour1, colour2, out var group))
        {
            return;
        }

        session.SendPacket(new PurchaseOKComposer());

        roomData.Group = group;

        session.User.Credits -= groupCost;
        session.SendPacket(new CreditBalanceComposer(session.User.Credits));

        if (session.User.RoomId != roomData.Id)
        {
            session.SendPacket(new RoomForwardComposer(roomData.Id));
        }

        session.SendPacket(new NewGroupInfoComposer(roomId, group.Id));
    }
}
