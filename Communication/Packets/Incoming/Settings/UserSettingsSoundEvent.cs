namespace WibboEmulator.Communication.Packets.Incoming.Settings;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class UserSettingsSoundEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        var volume1 = packet.PopInt();
        var volume2 = packet.PopInt();
        var volume3 = packet.PopInt();

        if (Session.User.ClientVolume[0] == volume1 && Session.User.ClientVolume[1] == volume2 && Session.User.ClientVolume[2] == volume3)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateVolume(dbClient, Session.User.Id, volume1, +volume2, +volume3);
        }

        Session.User.ClientVolume.Clear();
        Session.User.ClientVolume.Add(volume1);
        Session.User.ClientVolume.Add(volume2);
        Session.User.ClientVolume.Add(volume3);
    }
}
