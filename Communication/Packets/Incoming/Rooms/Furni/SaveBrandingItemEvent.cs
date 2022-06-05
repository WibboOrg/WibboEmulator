using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveBrandingItemEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int itemId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(itemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.ADS_BACKGROUND)
            {
                return;
            }

            int Data = Packet.PopInt();
            string text = Packet.PopString();
            string text2 = Packet.PopString();
            string text3 = Packet.PopString();
            string text4 = Packet.PopString();
            string text5 = Packet.PopString();
            string text6 = Packet.PopString();
            string text7 = Packet.PopString();
            string text8 = Packet.PopString();
            if (Data != 10 && Data != 8)
            {
                return;
            }

            string BrandData = string.Concat(new object[]
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
}
