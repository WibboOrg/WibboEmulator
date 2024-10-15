namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class DanceEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        roomUserByUserId.Unidle();
        var danceId = packet.PopInt();
        if (danceId is < 0 or > 4)
        {
            danceId = 0;
        }

        if (danceId > 0 && roomUserByUserId.CarryItemId > 0)
        {
            roomUserByUserId.CarryItem(0);
        }

        roomUserByUserId.DanceId = danceId;
        room.SendPacket(new DanceComposer(roomUserByUserId.VirtualId, danceId));
        QuestManager.ProgressUserQuest(session, QuestType.SocialDance, 0);
    }
}
