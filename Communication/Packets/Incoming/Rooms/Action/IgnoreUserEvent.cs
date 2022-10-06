namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class IgnoreUserEvent : IPacketEvent
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

        var UserName = Packet.PopString();

        var gameclient = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(UserName);
        if (gameclient == null)
        {
            return;
        }

        var user = gameclient.GetUser();
        if (user == null || session.GetUser().MutedUsers.Contains(user.Id))
        {
            return;
        }

        session.GetUser().MutedUsers.Add(user.Id);

        session.SendPacket(new IgnoreStatusComposer(1, UserName));
    }
}