using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplay;
using WibboEmulator.Games.Roleplay.Player;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.RolePlay
{
    internal class RpBuyItemsEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int Count = Packet.PopInt();

            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (Count > 99)
            {
                Count = 99;
            }

            if (Count < 1)
            {
                Count = 1;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
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

            RPItem RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
            if (RpItem == null)
            {
                return;
            }

            if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
            {
                User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itemown", Session.Langue));
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
                User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itempick", Session.Langue));
            }
            else
            {
                User.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itembuy", Session.Langue), RpItem.Price));
            }

            Rp.Money -= RpItem.Price * Count;
            Rp.SendUpdate();
        }
    }
}
