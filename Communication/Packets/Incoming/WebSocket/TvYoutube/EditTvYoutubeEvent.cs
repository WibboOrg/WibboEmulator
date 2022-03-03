using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;
using System;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class EditTvYoutubeEvent : IPacketWebEvent
    {
        public double Delay => 500;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Url = Packet.PopString();

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetUser() == null)
            {
                return;
            }

            Room room = Client.GetUser().CurrentRoom;
            if (room == null || !room.CheckRights(Client))
            {
                return;
            }

            Item item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.TVYOUTUBE)
            {
                return;
            }

            if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
            {                return;            }            string Split = "";            if (Url.Contains("?v="))            {                Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];            }            else if (Url.Contains("youtu.be/"))            {                Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];            }            if (Split.Length < 11)            {                return;            }            string VideoId = Split.Substring(0, 11);

            item.ExtraData = VideoId;
            item.UpdateState();
        }
    }
}
