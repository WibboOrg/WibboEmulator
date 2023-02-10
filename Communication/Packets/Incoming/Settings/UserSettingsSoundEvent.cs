namespace WibboEmulator.Communication.Packets.Incoming.Settings;

using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class UserSettingsSoundEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        var volume1 = packet.PopInt();
        var volume2 = packet.PopInt();
        var volume3 = packet.PopInt();

        if (session.User.ClientVolume[0] == volume1 && session.User.ClientVolume[1] == volume2 && session.User.ClientVolume[2] == volume3)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateVolume(dbClient, session.User.Id, volume1, +volume2, +volume3);
        }

        session.User.ClientVolume.Clear();
        session.User.ClientVolume.Add(volume1);
        session.User.ClientVolume.Add(volume2);
        session.User.ClientVolume.Add(volume3);
    }
}
