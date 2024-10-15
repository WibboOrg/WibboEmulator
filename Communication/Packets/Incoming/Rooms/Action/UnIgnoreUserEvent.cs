namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal sealed class UnIgnoreUserEvent : IPacketEvent
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

        var str = packet.PopString();

        var user = GameClientManager.GetClientByUsername(str).User;
        if (user == null || !Session.User.MutedUsers.Contains(user.Id))
        {
            return;
        }

        _ = Session.User.MutedUsers.Remove(user.Id);

        Session.SendPacket(new IgnoreStatusComposer(3, str));
    }
}
