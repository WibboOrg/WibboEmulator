namespace WibboEmulator.Communication.Packets.Incoming.Settings;

using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class UserSettingsSoundEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var volume1 = packet.PopInt();
        var volume2 = packet.PopInt();
        var volume3 = packet.PopInt();


        if (session.GetUser().ClientVolume[0] == volume1 && session.GetUser().ClientVolume[1] == volume2 && session.GetUser().ClientVolume[2] == volume3)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateVolume(dbClient, session.GetUser().Id, volume1, +volume2, +volume3);
        }

        session.GetUser().ClientVolume.Clear();
        session.GetUser().ClientVolume.Add(volume1);
        session.GetUser().ClientVolume.Add(volume2);
        session.GetUser().ClientVolume.Add(volume3);
    }
}
