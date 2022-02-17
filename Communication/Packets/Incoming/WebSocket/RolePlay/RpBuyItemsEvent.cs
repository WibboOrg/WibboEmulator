using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class RpBuyItemsEvent : IPacketWebEvent
    {
        public double Delay => 0;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int Count = Packet.PopInt();

            if (Count > 99)
            {
                Count = 99;
            }

            if (Count < 1)
            {
                Count = 1;
            }

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (!User.AllowBuyItems.Contains(ItemId))
            {
                return;
            }

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.Dead || Rp.SendPrison)
            {
                return;
            }

            RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
            if (RpItem == null)
            {
                return;
            }

            if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
            {
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.itemown", Client.Langue));
                return;
            }

            if (!RpItem.AllowStack && Count > 1)
            {
                Count = 1;
            }

            if (Rp.Money < (RpItem.Price * Count))
            {
                return;
            }

            Rp.AddInventoryItem(RpItem.Id, Count);

            if (RpItem.Price == 0)
            {
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.itempick", Client.Langue));
            }
            else
            {
                User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.itembuy", Client.Langue), RpItem.Price));
            }

            Rp.Money -= RpItem.Price * Count;
            Rp.SendUpdate();
        }
    }
}
