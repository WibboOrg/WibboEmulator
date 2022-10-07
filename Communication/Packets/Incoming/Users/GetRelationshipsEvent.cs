namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Relationships;

internal class GetRelationshipsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var user = WibboEnvironment.GetUserById(packet.PopInt());
        if (user == null)
        {
            return;
        }

        if (user.GetMessenger() == null)
        {
            session.SendPacket(new GetRelationshipsComposer(user.Id, new List<Relationship>()));
            return;
        }

        session.SendPacket(new GetRelationshipsComposer(user.Id, user.GetMessenger().GetRelationships()));
    }
}
