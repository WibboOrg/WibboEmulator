namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;

internal class UserSettingsRoomInvitesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var flag = Packet.PopBoolean();

        if (session == null || session.GetUser() == null)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateIgnoreRoomInvites(dbClient, session.GetUser().Id, flag);
        }

        session.GetUser().IgnoreRoomInvites = flag;
    }
}
