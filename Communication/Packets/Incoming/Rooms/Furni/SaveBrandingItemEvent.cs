using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveBrandingItemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.ADSBACKGROUND)
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
