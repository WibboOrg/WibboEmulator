namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class SetTonerEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(itemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.TONER)
        {
            return;
        }

        var num2 = packet.PopInt();
        var num3 = packet.PopInt();
        var num4 = packet.PopInt();

        roomItem.ExtraData = "on," + num2 + "," + num3 + "," + num4;
        roomItem.UpdateState(true, true);
    }
}
