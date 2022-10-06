namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class SetMannequinFigureEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var ItemId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(ItemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MANNEQUIN)
        {
            return;
        }

        var Look = "";
        foreach (var Part in session.GetUser().Look.Split('.'))
        {
            if (Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("cc") || Part.StartsWith("ca") || Part.StartsWith("sh") || Part.StartsWith("wa"))
            {
                Look = Look + Part + ".";
            }
        }

        Look = Look[..^1];
        if (Look.Length > 200)
        {
            Look = Look[..200];
        }

        var Stuff = roomItem.ExtraData.Split(new char[1] { ';' });
        var Name = "";

        if (Stuff.Length >= 3)
        {
            Name = Stuff[2];
        }

        roomItem.ExtraData = session.GetUser().Gender.ToUpper() + ";" + Look + ";" + Name;
        roomItem.UpdateState();
    }
}
