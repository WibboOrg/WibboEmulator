namespace WibboEmulator.Communication.Packets.Incoming.RolePlay.Troc;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays.Troc;

internal sealed class RpTrocRemoveItemEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.Room;
        if (room == null || !room.IsRoleplay)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
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
