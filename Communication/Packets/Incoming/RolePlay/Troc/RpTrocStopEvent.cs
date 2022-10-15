namespace WibboEmulator.Communication.Packets.Incoming.RolePlay.Troc;
using WibboEmulator.Games.GameClients;

internal class RpTrocStopEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
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

        WibboEnvironment.GetGame().GetRoleplayManager().TrocManager.RemoveTrade(rp.TradeId);
    }
}
