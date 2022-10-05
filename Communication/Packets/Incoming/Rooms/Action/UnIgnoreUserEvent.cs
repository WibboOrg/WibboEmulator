namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class UnIgnoreUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().CurrentRoom == null)
        {
            return;
        }

        var str = Packet.PopString();

        var user = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(str).GetUser();
        if (user == null || !session.GetUser().MutedUsers.Contains(user.Id))
        {
            return;
        }

        session.GetUser().MutedUsers.Remove(user.Id);

        session.SendPacket(new IgnoreStatusComposer(3, str));
    }
}
