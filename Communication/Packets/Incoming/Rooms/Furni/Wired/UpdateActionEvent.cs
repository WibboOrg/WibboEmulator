namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items.Wired;

internal class UpdateActionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var room = session.GetUser().CurrentRoom;
        if (room == null)
        {
            return;
        }

        if (!room.CheckRights(session) && !room.CheckRights(session, true))
        {
            return;
        }

        var itemId = packet.PopInt();

        var item = room.GetRoomItemHandler().GetItem(itemId);
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

        var delay = packet.PopInt();

        var selectionCode = packet.PopInt();

        var isStaff = session.GetUser().HasPermission("perm_superwired_staff");
        var isGod = session.GetUser().HasPermission("perm_superwired_god");

        WiredRegister.HandleRegister(item, room, intParams, stringParam, stuffIds, selectionCode, delay, isStaff, isGod);

        session.SendPacket(new SaveWiredComposer());
    }
}
