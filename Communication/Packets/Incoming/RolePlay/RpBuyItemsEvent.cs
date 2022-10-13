namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Games.GameClients;

internal class RpBuyItemsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var count = packet.PopInt();

        if (session == null || session.GetUser() == null)
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

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.IsRoleplay)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
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

        var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(itemId);
        if (rpItem == null)
        {
            return;
        }

        if (!rpItem.AllowStack && rp.GetInventoryItem(rpItem.Id) != null)
        {
            user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itemown", session.Langue));
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
            user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itempick", session.Langue));
        }
        else
        {
            user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itembuy", session.Langue), rpItem.Price));
        }

        rp.Money -= rpItem.Price * count;
        rp.SendUpdate();
    }
}
