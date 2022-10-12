namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class RespectUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (session.GetUser().DailyRespectPoints <= 0)
        {
            return;
        }

        var roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(packet.PopInt());
        if (roomUserByUserIdTarget == null || roomUserByUserIdTarget.Client.GetUser().Id == session.GetUser().Id || roomUserByUserIdTarget.IsBot)
        {
            return;
        }

        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SOCIAL_RESPECT, 0);
        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserIdTarget.Client, "ACH_RespectEarned", 1);
        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RespectGiven", 1);
        session.GetUser().DailyRespectPoints--;
        roomUserByUserIdTarget.Client.GetUser().Respect++;

        room.SendPacket(new RespectNotificationComposer(roomUserByUserIdTarget.Client.GetUser().Id, roomUserByUserIdTarget.Client.GetUser().Respect));

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);

        room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, 7));
    }
}