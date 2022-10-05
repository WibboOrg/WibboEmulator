namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class SetTonerEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var ItemId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(ItemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.TONER)
        {
            return;
        }

        var num2 = Packet.PopInt();
        var num3 = Packet.PopInt();
        var num4 = Packet.PopInt();

        roomItem.ExtraData = "on," + num2 + "," + num3 + "," + num4;
        roomItem.UpdateState(true, true);
    }
}