namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class UpdateStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT)
        {
            return;
        }

        var Color = Packet.PopString();
        var Message = Packet.PopString();

        if (!room.CheckRights(session) && !Message.StartsWith(roomItem.ExtraData))
        {
            return;
        }

        switch (Color)
        {
            case "FFFF33":
            case "FF9CFF":
            case "9CCEFF":
            case "9CFF9C":
                roomItem.ExtraData = Color + " " + Message;
                roomItem.UpdateState(true, true);
                break;
        }

    }
}
