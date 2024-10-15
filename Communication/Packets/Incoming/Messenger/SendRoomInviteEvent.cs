namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class SendRoomInviteEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
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

        var textMessage = packet.PopString(121);

        if (Session.User.CheckChatMessage(textMessage, "<RM>"))
        {
            return;
        }

        if (Session.User.IgnoreAll)
        {
            return;
        }


        ServerPacket roomInvitePacket = new RoomInviteComposer(Session.User.Id, textMessage);

        foreach (var userId in targets)
        {
            if (Session.User.Messenger.FriendshipExists(userId))
            {
                var clientByUserId = GameClientManager.GetClientByUserID(userId);
                if (clientByUserId == null || clientByUserId.User.IgnoreRoomInvites)
                {
                    break;
                }

                if (clientByUserId.User.Messenger.FriendshipExists(Session.User.Id))
                {
                    clientByUserId.SendPacket(roomInvitePacket);
                }
            }
        }
    }
}
