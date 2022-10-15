namespace WibboEmulator.Communication.Packets.Incoming.RolePlay.Troc;
using WibboEmulator.Games.GameClients;

internal class RpTrocRemoveItemEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.CurrentRoom;
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

        WibboEnvironment.GetGame().GetRoleplayManager().TrocManager.RemoveItem(rp.TradeId, user.UserId, itemId);
    }
}
