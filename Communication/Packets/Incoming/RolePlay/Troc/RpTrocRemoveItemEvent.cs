namespace WibboEmulator.Communication.Packets.Incoming.RolePlay.Troc;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays.Troc;

internal sealed class RpTrocRemoveItemEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (Session == null || Session.User == null)
        {
            return;
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

        var rp = user.Roleplayer;
        if (rp == null || rp.TradeId == 0)
        {
            return;
        }

        RPTrocManager.RemoveItem(rp.TradeId, user.UserId, itemId);
    }
}
