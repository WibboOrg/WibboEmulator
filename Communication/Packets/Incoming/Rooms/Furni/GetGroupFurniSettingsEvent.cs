namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class GetGroupFurniSettingsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        var itemId = packet.PopInt();
        var groupId = packet.PopInt();

        var item = session.User.CurrentRoom.RoomItemHandling.GetItem(itemId);
        if (item == null)
        {
            return;
        }

        if (item.Data.InteractionType != InteractionType.GUILD_GATE)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        session.SendPacket(new GroupFurniSettingsComposer(group, itemId, session.User.Id));
        session.SendPacket(new GroupInfoComposer(group, session, false));
    }
}
