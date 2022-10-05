namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class ChangeFootGate : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var Id = Packet.PopInt();
        var gender = Packet.PopString();
        var look = Packet.PopString();

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        var item = room.GetRoomItemHandler().GetItem(Id);
        if (item == null || item.GetBaseItem().InteractionType != InteractionType.FBGATE)
        {
            return;
        }

        if (gender.ToUpper() == "M")
        {
            var Figures = item.ExtraData.Split(',');
            var newFigures = new string[2];

            newFigures[0] = look;
            if (Figures.Length > 1)
            {
                newFigures[1] = Figures[1];
            }
            else
            {
                newFigures[1] = "hd-99999-99999.ch-630-62.lg-695-62";
            }

            item.ExtraData = string.Join(",", newFigures);
        }
        else if (gender.ToUpper() == "F")
        {
            var Figures = item.ExtraData.Split(',');
            var newFigures = new string[2];

            if (!string.IsNullOrWhiteSpace(Figures[0]))
            {
                newFigures[0] = Figures[0];
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