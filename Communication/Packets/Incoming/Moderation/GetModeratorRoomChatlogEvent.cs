namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GetModeratorRoomChatlogEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("mod"))
        {
            return;
        }

        _ = packet.PopInt(); //useless
        var roomId = packet.PopInt();

        if (!RoomManager.TryGetRoom(roomId, out var room))
        {
            return;
        }

        var listReverse = new List<ChatlogEntry>();
        listReverse.AddRange(room.ChatlogManager.ListOfMessages);
        listReverse.Reverse();

        Session.SendPacket(new ModeratorRoomChatlogComposer(room, listReverse));
    }
}
