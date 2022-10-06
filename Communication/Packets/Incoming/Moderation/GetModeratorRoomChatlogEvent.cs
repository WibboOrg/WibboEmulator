namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.GameClients;

internal class GetModeratorRoomChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        Packet.PopInt(); //useless
        var roomId = Packet.PopInt();

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
