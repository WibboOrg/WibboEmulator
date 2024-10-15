namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Games.GameClients;

internal sealed class ScrGetUserInfoMessageEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        Session.User.Premium.SendPackets(true);
    }
}
