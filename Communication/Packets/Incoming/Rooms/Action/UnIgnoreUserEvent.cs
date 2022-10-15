namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class UnIgnoreUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (session.User.CurrentRoom == null)
        {
            return;
        }

        var str = packet.PopString();

        var user = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(str).User;
        if (user == null || !session.User.MutedUsers.Contains(user.Id))
        {
            return;
        }

        _ = session.User.MutedUsers.Remove(user.Id);

        session.SendPacket(new IgnoreStatusComposer(3, str));
    }
}
