namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class SetTonerEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.ItemData.InteractionType != InteractionType.TONER)
        {
            return;
        }

        var num2 = packet.PopInt();
        var num3 = packet.PopInt();
        var num4 = packet.PopInt();

        roomItem.ExtraData = "on," + num2 + "," + num3 + "," + num4;
        roomItem.UpdateState();
    }
}
