namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class SaveBrandingItemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var itemId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var roomItem = room.GetRoomItemHandler().GetItem(itemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.ADS_BACKGROUND)
        {
            return;
        }

        var Data = Packet.PopInt();
        var text = Packet.PopString();
        var text2 = Packet.PopString();
        var text3 = Packet.PopString();
        var text4 = Packet.PopString();
        var text5 = Packet.PopString();
        var text6 = Packet.PopString();
        var text7 = Packet.PopString();
        var text8 = Packet.PopString();
        if (Data is not 10 and not 8)
        {
            return;
        }

        var BrandData = string.Concat(new object[]
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

        roomItem.ExtraData = BrandData;
        roomItem.UpdateState();
    }
}
