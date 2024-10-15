namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class ChangeFootGate : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var id = packet.PopInt();
        var gender = packet.PopString(1);
        var look = packet.PopString();

        var room = Session.User.Room;
        if (room == null || !room.CheckRights(Session, true))
        {
            return;
        }

        var item = room.RoomItemHandling.GetItem(id);
        if (item == null || item.ItemData.InteractionType != InteractionType.FOOTBALL_GATE)
        {
            return;
        }

        if (gender.Equals("M", StringComparison.CurrentCultureIgnoreCase))
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
        else if (gender.Equals("F", StringComparison.CurrentCultureIgnoreCase))
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
