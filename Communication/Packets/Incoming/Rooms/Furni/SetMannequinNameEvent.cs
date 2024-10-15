namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class SetMannequinNameEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var name = packet.PopString(100);

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.ItemData.InteractionType != InteractionType.MANNEQUIN)
        {
            return;
        }

        var allowedParts = new List<string> { "ha", "he", "ea", "ch", "fa", "cp", "lg", "cc", "ca", "sh", "wa" };
        var look = string.Join(".", Session.User.Look.Split('.').Where(part => allowedParts.Contains(part.Split('-')[0])));

        name = name.Replace(";", ":");

        roomItem.ExtraData = Session.User.Gender + ";" + look + ";" + name;
        roomItem.UpdateState();
    }
}
