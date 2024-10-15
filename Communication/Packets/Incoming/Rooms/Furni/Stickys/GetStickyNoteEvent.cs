namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class GetStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var itemId = packet.PopInt();

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.ItemData.InteractionType != InteractionType.POSTIT)
        {
            return;
        }

        Session.SendPacket(new StickyNoteComposer(roomItem.Id, roomItem.ExtraData));
    }
}
