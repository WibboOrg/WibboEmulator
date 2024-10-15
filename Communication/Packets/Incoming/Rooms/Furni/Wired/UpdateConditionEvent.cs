namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Wired;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items.Wired;

internal sealed class UpdateConditionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var room = session.User.Room;
        if (room == null)
        {
            return;
        }

        if (!room.CheckRights(session) && !room.CheckRights(session, true))
        {
            return;
        }

        var itemId = packet.PopInt();

        var item = room.RoomItemHandling.GetItem(itemId);
        if (item == null)
        {
            return;
        }

        var intParams = new List<int>();
        var countInt = packet.PopInt();
        for (var i = 0; i < countInt; i++)
        {
            intParams.Add(packet.PopInt());
        }

        var stringParam = packet.PopString();

        var stuffIds = new List<int>();
        var countStuff = packet.PopInt();
        for (var i = 0; i < countStuff; i++)
        {
            stuffIds.Add(packet.PopInt());
        }

        var selectionCode = packet.PopInt();

        var isStaff = session.User.HasPermission("superwired_staff");
        var isGod = session.User.HasPermission("superwired_god");

        WiredRegister.HandleRegister(item, room, intParams, stringParam, stuffIds, selectionCode, 0, isStaff, isGod);

        session.SendPacket(new SaveWiredComposer());
    }
}
