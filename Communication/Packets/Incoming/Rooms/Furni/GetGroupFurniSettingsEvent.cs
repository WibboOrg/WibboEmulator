namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;

internal sealed class GetGroupFurniSettingsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom)
        {
            return;
        }

        var itemId = packet.PopInt();
        var groupId = packet.PopInt();

        var item = Session.User.Room.RoomItemHandling.GetItem(itemId);
        if (item == null)
        {
            return;
        }

        if (item.Data.InteractionType != InteractionType.GUILD_GATE)
        {
            return;
        }

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        Session.SendPacket(new GroupFurniSettingsComposer(group, itemId, Session.User.Id));
        Session.SendPacket(new GroupInfoComposer(group, Session, false));
    }
}
