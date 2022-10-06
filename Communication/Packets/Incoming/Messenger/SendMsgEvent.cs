namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;

internal class SendMsgEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var userId = Packet.PopInt();

        if (userId == session.GetUser().Id)
        {
            return;
        }

        var Message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
        if (string.IsNullOrWhiteSpace(Message))
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

        if (session.Antipub("<" + userId + "> " + Message, "<MP>"))
        {
            return;
        }

        if (session.GetUser().IgnoreAll)
        {
            return;
        }

        session.GetUser().GetMessenger().SendInstantMessage(userId, Message);
    }
}