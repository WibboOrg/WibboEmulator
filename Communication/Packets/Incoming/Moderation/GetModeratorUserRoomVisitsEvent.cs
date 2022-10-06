namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;

internal class GetModeratorUserRoomVisitsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var userId = Packet.PopInt();

        var clientTarget = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);

        if (clientTarget == null)
        {
            return;
        }

        session.SendPacket(new ModeratorUserRoomVisitsComposer(clientTarget.GetUser(), clientTarget.GetUser().Visits));
    }
}
