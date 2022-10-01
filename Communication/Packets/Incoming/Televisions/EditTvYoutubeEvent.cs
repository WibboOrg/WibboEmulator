using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Televisions
{
    internal class EditTvYoutubeEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Url = Packet.PopString();

            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            Room room = Session.GetUser().CurrentRoom;
            if (room == null || !room.CheckRights(Session))
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
