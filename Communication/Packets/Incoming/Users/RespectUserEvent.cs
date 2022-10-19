namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class RespectUserEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (session.User.DailyRespectPoints <= 0)
        {
            return;
        }

        var roomUserByUserIdTarget = room.RoomUserManager.GetRoomUserByUserId(packet.PopInt());
        if (roomUserByUserIdTarget == null || roomUserByUserIdTarget.Client.User.Id == session.User.Id || roomUserByUserIdTarget.IsBot)
        {
            return;
        }

        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialRespect, 0);
        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserIdTarget.Client, "ACH_RespectEarned", 1);
        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RespectGiven", 1);
        session.User.DailyRespectPoints--;
        roomUserByUserIdTarget.Client.User.Respect++;

        room.SendPacket(new RespectNotificationComposer(roomUserByUserIdTarget.Client.User.Id, roomUserByUserIdTarget.Client.User.Respect));

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);

        room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, 7));
    }
}
