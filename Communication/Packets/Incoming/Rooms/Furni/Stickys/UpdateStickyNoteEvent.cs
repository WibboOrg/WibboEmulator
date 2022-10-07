namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class UpdateStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(packet.PopInt());
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT)
        {
            return;
        }

        var color = packet.PopString();
        var message = packet.PopString();

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
                roomItem.UpdateState(true, true);
                break;
        }

    }
}
