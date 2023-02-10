namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class SaveBrandingItemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.ADS_BACKGROUND)
        {
            return;
        }

        var data = packet.PopInt();
        var text = packet.PopString();
        var text2 = packet.PopString();
        var text3 = packet.PopString();
        var text4 = packet.PopString();
        var text5 = packet.PopString();
        var text6 = packet.PopString();
        var text7 = packet.PopString();
        var text8 = packet.PopString();
        if (data is not 10 and not 8)
        {
            return;
        }

        var brandData = string.Concat(new object[]
                {
                    text.Replace("=", ""),
                    "=",
                    text2.Replace("=", ""),
                    Convert.ToChar(9),
                    text3.Replace("=", ""),
                    "=",
                    text4.Replace("=", ""),
                    Convert.ToChar(9),
                    text5.Replace("=", ""),
                    "=",
                    text6.Replace("=", ""),
                    Convert.ToChar(9),
                    text7.Replace("=", ""),
                    "=",
                    text8.Replace("=", "")
                });

        roomItem.ExtraData = brandData;
        roomItem.UpdateState();
    }
}
