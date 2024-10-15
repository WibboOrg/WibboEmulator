namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal sealed class IgnoreUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (Session.User.Room == null)
        {
            return;
        }

        var userName = packet.PopString(16);

        var gameclient = GameClientManager.GetClientByUsername(userName);
        if (gameclient == null)
        {
            return;
        }

        var user = gameclient.User;
        if (user == null || Session.User.MutedUsers.Contains(user.Id))
        {
            return;
        }

        Session.User.MutedUsers.Add(user.Id);

        Session.SendPacket(new IgnoreStatusComposer(1, userName));
    }
}
