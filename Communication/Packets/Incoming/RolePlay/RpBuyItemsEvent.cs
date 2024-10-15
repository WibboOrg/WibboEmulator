namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays.Item;

internal sealed class RpBuyItemsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var count = packet.PopInt();

        if (Session == null || Session.User == null)
        {
            return;
        }

        if (count > 99)
        {
            count = 99;
        }

        if (count < 1)
        {
            count = 1;
        }

        var room = Session.User.Room;
        if (room == null || !room.IsRoleplay)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (user == null)
        {
            return;
        }

        if (!user.AllowBuyItems.Contains(itemId))
        {
            return;
        }

        var rp = user.Roleplayer;
        if (rp == null || rp.Dead || rp.SendPrison)
        {
            return;
        }

        var rpItem = RPItemManager.GetItem(itemId);
        if (rpItem == null)
        {
            return;
        }

        if (!rpItem.AllowStack && rp.GetInventoryItem(rpItem.Id) != null)
        {
            user.SendWhisperChat(LanguageManager.TryGetValue("rp.itemown", Session.Language));
            return;
        }

        if (!rpItem.AllowStack && count > 1)
        {
            count = 1;
        }

        if (rp.Money < (rpItem.Price * count))
        {
            return;
        }

        rp.AddInventoryItem(rpItem.Id, count);

        if (rpItem.Price == 0)
        {
            user.SendWhisperChat(LanguageManager.TryGetValue("rp.itempick", Session.Language));
        }
        else
        {
            user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("rp.itembuy", Session.Language), rpItem.Price));
        }

        rp.Money -= rpItem.Price * count;
        rp.SendUpdate();
    }
}
