namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class SendMsgEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || session.User.Messenger == null)
        {
            return;
        }

        var userId = packet.PopInt();

        if (userId == session.User.Id)
        {
            return;
        }

        var message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var timeSpan = DateTime.Now - session.User.FloodTime;
        if (timeSpan.Seconds > 4)
        {
            session.User.FloodCount = 0;
        }

        if (timeSpan.Seconds < 4 && session.User.FloodCount > 5 && session.User.Rank < 5)
        {
            return;
        }

        session.User.FloodTime = DateTime.Now;
        session.User.FloodCount++;

        if (session.User.Antipub("<" + userId + "> " + message, "<MP>"))
        {
            return;
        }

        if (session.User.IgnoreAll)
        {
            return;
        }

        session.User.Messenger.SendInstantMessage(userId, message);
    }
}
