namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal class SendRoomInviteEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
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

        var inviteCount = Packet.PopInt();
        if (inviteCount > 100)
        {
            return;
        }

        var targets = new List<int>();
        for (var i = 0; i < inviteCount; ++i)
        {
            var Id = Packet.PopInt();
            if (i < 100)
            {
                targets.Add(Id);
            }
        }

        var textMessage = StringCharFilter.Escape(Packet.PopString());
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

        foreach (var UserId in targets)
        {
            if (session.GetUser().GetMessenger().FriendshipExists(UserId))
            {
                var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(UserId);
                if (clientByUserId == null || clientByUserId.GetUser().IgnoreRoomInvites)
                {
                    break;
                }

                if (clientByUserId.GetUser().GetMessenger().FriendshipExists(session.GetUser().Id))
                {
                    clientByUserId.SendPacket(roomInvitePacket);
                }
            }
        }
    }
}
