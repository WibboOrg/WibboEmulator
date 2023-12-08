namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class SetMannequinNameEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var name = packet.PopString(100);

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MANNEQUIN)
        {
            return;
        }

        var look = "";
        foreach (var part in session.User.Look.Split('.'))
        {
            if (part.StartsWith("ch") || part.StartsWith("lg") || part.StartsWith("cc") || part.StartsWith("ca") || part.StartsWith("sh") || part.StartsWith("wa"))
            {
                look = look + part + ".";
            }
        }

        look = look[..^1];

        name = name.Replace(";", ":");

        roomItem.ExtraData = session.User.Gender + ";" + look + ";" + name;
        roomItem.UpdateState();
    }
}
