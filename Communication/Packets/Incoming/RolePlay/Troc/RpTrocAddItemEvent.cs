namespace WibboEmulator.Communication.Packets.Incoming.RolePlay.Troc;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RpTrocAddItemEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var ItemId = packet.PopInt();

        if (session == null || session.GetUser() == null)
        {
            return;
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

        var Rp = User.Roleplayer;
        if (Rp == null || Rp.TradeId == 0)
        {
            return;
        }

        WibboEnvironment.GetGame().GetRoleplayManager().GetTrocManager().AddItem(Rp.TradeId, User.UserId, ItemId);
    }
}
