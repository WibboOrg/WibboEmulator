namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Messenger;

internal sealed class GetRelationshipsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();

        var user = WibboEnvironment.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        if (user.Messenger == null)
        {
            session.SendPacket(new GetRelationshipsComposer(user.Id, []));
            return;
        }

        session.SendPacket(new GetRelationshipsComposer(user.Id, user.Messenger.Relationships));
    }
}
