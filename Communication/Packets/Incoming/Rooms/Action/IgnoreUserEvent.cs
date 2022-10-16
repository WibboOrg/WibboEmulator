namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class IgnoreUserEvent : IPacketEvent
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

        var userName = packet.PopString();

        var gameclient = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(userName);
        if (gameclient == null)
        {
            return;
        }

        var user = gameclient.User;
        if (user == null || session.User.MutedUsers.Contains(user.Id))
        {
            return;
        }

        session.User.MutedUsers.Add(user.Id);

        session.SendPacket(new IgnoreStatusComposer(1, userName));
    }
}
