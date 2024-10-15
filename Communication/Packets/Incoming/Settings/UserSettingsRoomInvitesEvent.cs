namespace WibboEmulator.Communication.Packets.Incoming.Settings;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class UserSettingsRoomInvitesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var flag = packet.PopBoolean();

        if (Session == null || Session.User == null)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateIgnoreRoomInvites(dbClient, Session.User.Id, flag);
        }

        Session.User.IgnoreRoomInvites = flag;
    }
}
