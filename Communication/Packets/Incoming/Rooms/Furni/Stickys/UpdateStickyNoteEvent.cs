namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class UpdateStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session))
        {
            return;
        }

        var itemId = packet.PopInt();
        var color = packet.PopString();
        var message = packet.PopString();

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.ItemData.InteractionType != InteractionType.POSTIT)
        {
            return;
        }

        if (!room.CheckRights(Session) && !message.StartsWith(roomItem.ExtraData))
        {
            return;
        }

        switch (color)
        {
            case "FFFF33":
            case "FF9CFF":
            case "9CCEFF":
            case "9CFF9C":
                roomItem.ExtraData = color + " " + message;
                roomItem.UpdateState();
                break;
        }

    }
}
