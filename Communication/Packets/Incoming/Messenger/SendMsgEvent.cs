namespace WibboEmulator.Communication.Packets.Incoming.Messenger;

using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.GameClients;

internal sealed class SendMsgEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || Session.User.Messenger == null)
        {
            return;
        }

        var userId = packet.PopInt();

        if (userId == Session.User.Id)
        {
            return;
        }

        var message = WordFilterManager.CheckMessage(packet.PopString());
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var timeSpan = DateTime.Now - Session.User.FloodTime;
        if (timeSpan.Seconds > 4)
        {
            Session.User.FloodCount = 0;
        }

        if (timeSpan.Seconds < 4 && Session.User.FloodCount > 5 && Session.User.Rank < 5)
        {
            return;
        }

        Session.User.FloodTime = DateTime.Now;
        Session.User.FloodCount++;

        if (Session.User.CheckChatMessage("<" + userId + "> " + message, "<MP>"))
        {
            return;
        }

        if (Session.User.IgnoreAll)
        {
            return;
        }

        Session.User.Messenger.SendInstantMessage(userId, message);
    }
}
