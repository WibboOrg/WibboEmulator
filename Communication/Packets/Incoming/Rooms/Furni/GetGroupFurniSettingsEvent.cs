namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class GetGroupFurniSettingsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
        {
            return;
        }

        var ItemId = Packet.PopInt();
        var GroupId = Packet.PopInt();

        var Item = session.GetUser().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
        if (Item == null)
        {
            return;
        }

        if (Item.Data.InteractionType != InteractionType.GUILD_GATE)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        session.SendPacket(new GroupFurniSettingsComposer(Group, ItemId, session.GetUser().Id));
        session.SendPacket(new GroupInfoComposer(Group, session, false));
    }
}