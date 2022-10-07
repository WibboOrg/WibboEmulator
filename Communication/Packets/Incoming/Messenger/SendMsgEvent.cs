namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal class SendMsgEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null || session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var userId = packet.PopInt();

        if (userId == session.GetUser().Id)
        {
            return;
        }

        var message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var timeSpan = DateTime.Now - session.GetUser().FloodTime;
        if (timeSpan.Seconds > 4)
        {
            session.GetUser().FloodCount = 0;
        }

        if (timeSpan.Seconds < 4 && session.GetUser().FloodCount > 5 && session.GetUser().Rank < 5)
        {
            return;
        }

        session.GetUser().FloodTime = DateTime.Now;
        session.GetUser().FloodCount++;

        if (session.Antipub("<" + userId + "> " + message, "<MP>"))
        {
            return;
        }

        if (session.GetUser().IgnoreAll)
        {
            return;
        }

        session.GetUser().GetMessenger().SendInstantMessage(userId, message);
    }
}
