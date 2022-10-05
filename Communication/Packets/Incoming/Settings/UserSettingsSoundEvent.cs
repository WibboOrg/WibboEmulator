namespace WibboEmulator.Communication.Packets.Incoming.Structure;

using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;

internal class UserSettingsSoundEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var Volume1 = Packet.PopInt();
        var Volume2 = Packet.PopInt();
        var Volume3 = Packet.PopInt();


        if (session.GetUser().ClientVolume[0] == Volume1 && session.GetUser().ClientVolume[1] == Volume2 && session.GetUser().ClientVolume[2] == Volume3)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateVolume(dbClient, session.GetUser().Id, Volume1, +Volume2, +Volume3);
        }

        session.GetUser().ClientVolume.Clear();
        session.GetUser().ClientVolume.Add(Volume1);
        session.GetUser().ClientVolume.Add(Volume2);
        session.GetUser().ClientVolume.Add(Volume3);
    }
}
