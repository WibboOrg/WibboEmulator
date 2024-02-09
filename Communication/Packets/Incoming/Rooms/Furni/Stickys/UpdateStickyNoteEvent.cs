namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class UpdateStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var itemId = packet.PopInt();
        var color = packet.PopString();
        var message = packet.PopString();

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT)
        {
            return;
        }

        if (!room.CheckRights(session) && !message.StartsWith(roomItem.ExtraData))
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
