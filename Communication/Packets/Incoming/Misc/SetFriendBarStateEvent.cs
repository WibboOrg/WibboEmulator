namespace WibboEmulator.Communication.Packets.Incoming.Misc;
using WibboEmulator.Games.GameClients;

internal class SetFriendBarStateEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }
    }
}
