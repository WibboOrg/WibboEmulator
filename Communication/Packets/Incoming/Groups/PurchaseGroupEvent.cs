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

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var name = WordFilterManager.CheckMessage(packet.PopString(16));
        var description = WordFilterManager.CheckMessage(packet.PopString());
        var roomId = packet.PopInt();
        var colour1 = packet.PopInt();
        var colour2 = packet.PopInt();

        _ = packet.PopInt();

        var groupCost = 20;

        if (Session.User.Credits < groupCost)
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
        if (roomData == null || roomData.OwnerId != Session.User.Id || roomData.Group != null)
        {
            return;
        }

        var badge = string.Empty;

        for (var i = 0; i < 5; i++)
        {
            badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
        }

        if (!GroupManager.TryCreateGroup(Session.User, name, description, roomId, badge, colour1, colour2, out var group))
        {
            return;
        }

        Session.SendPacket(new PurchaseOKComposer());

        roomData.Group = group;

        Session.User.Credits -= groupCost;
        Session.SendPacket(new CreditBalanceComposer(Session.User.Credits));

        if (Session.User.RoomId != roomData.Id)
        {
            Session.SendPacket(new RoomForwardComposer(roomData.Id));
        }

        Session.SendPacket(new NewGroupInfoComposer(roomId, group.Id));
    }
}
