namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class ActionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        roomUserByUserId.Unidle();
        var actionId = packet.PopInt();

        room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, actionId));

        if (actionId == 5)
        {
            roomUserByUserId.IsAsleep = true;
            room.SendPacket(new SleepComposer(roomUserByUserId.VirtualId, true));
        }

        QuestManager.ProgressUserQuest(Session, QuestType.SocialWave, 0);
    }
}
