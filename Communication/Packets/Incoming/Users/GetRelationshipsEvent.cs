namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Users.Relationships;

internal class GetRelationshipsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var User = WibboEnvironment.GetUserById(packet.PopInt());
        if (User == null)
        {
            return;
        }

        if (User.GetMessenger() == null)
        {
            session.SendPacket(new GetRelationshipsComposer(User.Id, new List<Relationship>()));
            return;
        }

        session.SendPacket(new GetRelationshipsComposer(User.Id, User.GetMessenger().GetRelationships()));
    }
}
