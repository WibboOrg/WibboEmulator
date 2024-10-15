namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal sealed class GetRelationshipsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var userId = packet.PopInt();

        var user = UserManager.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        if (user.Messenger == null)
        {
            Session.SendPacket(new GetRelationshipsComposer(user.Id, []));
            return;
        }

        Session.SendPacket(new GetRelationshipsComposer(user.Id, user.Messenger.Relationships));
    }
}
