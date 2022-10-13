namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal class SendRoomInviteEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
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

        var inviteCount = packet.PopInt();
        if (inviteCount > 100)
        {
            return;
        }

        var targets = new List<int>();
        for (var i = 0; i < inviteCount; ++i)
        {
            var id = packet.PopInt();
            if (i < 100)
            {
                targets.Add(id);
            }
        }

        var textMessage = StringCharFilter.Escape(packet.PopString());
        if (textMessage.Length > 121)
        {
            textMessage = textMessage[..121];
        }

        if (session.Antipub(textMessage, "<RM>"))
        {
            return;
        }

        if (session.GetUser().IgnoreAll)
        {
            return;
        }


        ServerPacket roomInvitePacket = new RoomInviteComposer(session.GetUser().Id, textMessage);

        foreach (var userId in targets)
        {
            if (session.GetUser().Messenger.FriendshipExists(userId))
            {
                var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
                if (clientByUserId == null || clientByUserId.GetUser().IgnoreRoomInvites)
                {
                    break;
                }

                if (clientByUserId.GetUser().Messenger.FriendshipExists(session.GetUser().Id))
                {
                    clientByUserId.SendPacket(roomInvitePacket);
                }
            }
        }
    }
}
