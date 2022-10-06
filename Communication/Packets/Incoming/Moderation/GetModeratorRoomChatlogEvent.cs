namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.GameClients;

internal class GetModeratorRoomChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        _ = packet.PopInt(); //useless
        var roomId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return;
        }

        var listReverse = new List<ChatlogEntry>();
        listReverse.AddRange(room.GetChatMessageManager().ListOfMessages);
        listReverse.Reverse();

        session.SendPacket(new ModeratorRoomChatlogComposer(room, listReverse));
    }
}
