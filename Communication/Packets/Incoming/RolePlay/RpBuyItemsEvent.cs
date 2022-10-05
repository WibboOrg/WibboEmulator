namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RpBuyItemsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var ItemId = Packet.PopInt();
        var Count = Packet.PopInt();

        if (session == null || session.GetUser() == null)
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

        var Room = session.GetUser().CurrentRoom;
        if (Room == null || !Room.IsRoleplay)
        {
            return;
        }

        var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (User == null)
        {
            return;
        }

        if (!User.AllowBuyItems.Contains(ItemId))
        {
            return;
        }

        var Rp = User.Roleplayer;
        if (Rp == null || Rp.Dead || Rp.SendPrison)
        {
            return;
        }

        var RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
        if (RpItem == null)
        {
            return;
        }

        if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
        {
            User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itemown", session.Langue));
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
            User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itempick", session.Langue));
        }
        else
        {
            User.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itembuy", session.Langue), RpItem.Price));
        }

        Rp.Money -= RpItem.Price * Count;
        Rp.SendUpdate();
    }
}
