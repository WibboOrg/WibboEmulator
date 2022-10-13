namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class ChangeFootGate : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var id = packet.PopInt();
        var gender = packet.PopString();
        var look = packet.PopString();

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        var item = room.RoomItemHandling.GetItem(id);
        if (item == null || item.GetBaseItem().InteractionType != InteractionType.FBGATE)
        {
            return;
        }

        if (gender.ToUpper() == "M")
        {
            var figures = item.ExtraData.Split(',');
            var newFigures = new string[2];

            newFigures[0] = look;
            if (figures.Length > 1)
            {
                newFigures[1] = figures[1];
            }
            else
            {
                newFigures[1] = "hd-99999-99999.ch-630-62.lg-695-62";
            }

            item.ExtraData = string.Join(",", newFigures);
        }
        else if (gender.ToUpper() == "F")
        {
            var figures = item.ExtraData.Split(',');
            var newFigures = new string[2];

            if (!string.IsNullOrWhiteSpace(figures[0]))
            {
                newFigures[0] = figures[0];
            }
            else
            {
                newFigures[0] = "hd-99999-99999.lg-270-62";
            }
            newFigures[1] = look;

            item.ExtraData = string.Join(",", newFigures);
        }
    }
}
