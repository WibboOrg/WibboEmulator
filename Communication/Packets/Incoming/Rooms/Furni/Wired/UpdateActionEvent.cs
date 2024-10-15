namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Wired;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items.Wired;

internal sealed class UpdateActionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        if (!room.CheckRights(Session) && !room.CheckRights(Session, true))
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

        var delay = packet.PopInt();

        var selectionCode = packet.PopInt();

        var isStaff = Session.User.HasPermission("superwired_staff");
        var isGod = Session.User.HasPermission("superwired_god");

        WiredRegister.HandleRegister(item, room, intParams, stringParam, stuffIds, selectionCode, delay, isStaff, isGod);

        Session.SendPacket(new SaveWiredComposer());
    }
}
