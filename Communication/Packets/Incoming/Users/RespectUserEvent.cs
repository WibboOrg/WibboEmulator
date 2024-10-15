namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class RespectUserEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (Session.User.DailyRespectPoints <= 0)
        {
            return;
        }

        var roomUserByUserIdTarget = room.RoomUserManager.GetRoomUserByUserId(packet.PopInt());
        if (roomUserByUserIdTarget == null || roomUserByUserIdTarget.Client == null || roomUserByUserIdTarget.Client.User.Id == Session.User.Id || roomUserByUserIdTarget.IsBot)
        {
            return;
        }

        QuestManager.ProgressUserQuest(Session, QuestType.SocialRespect, 0);
        _ = AchievementManager.ProgressAchievement(roomUserByUserIdTarget.Client, "ACH_RespectEarned", 1);
        _ = AchievementManager.ProgressAchievement(Session, "ACH_RespectGiven", 1);
        Session.User.DailyRespectPoints--;
        roomUserByUserIdTarget.Client.User.Respect++;

        room.SendPacket(new RespectNotificationComposer(roomUserByUserIdTarget.Client.User.Id, roomUserByUserIdTarget.Client.User.Respect));

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);

        room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, 7));
    }
}
