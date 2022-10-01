using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RespectUserEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (Session.GetUser().DailyRespectPoints <= 0)
            {
                return;
            }

            RoomUser roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(Packet.PopInt());
            if (roomUserByUserIdTarget == null || roomUserByUserIdTarget.GetClient().GetUser().Id == Session.GetUser().Id || roomUserByUserIdTarget.IsBot)
            {
                return;
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT, 0);
            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserIdTarget.GetClient(), "ACH_RespectEarned", 1);
            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            Session.GetUser().DailyRespectPoints--;
            roomUserByUserIdTarget.GetClient().GetUser().Respect++;

            room.SendPacket(new RespectNotificationComposer(roomUserByUserIdTarget.GetClient().GetUser().Id, roomUserByUserIdTarget.GetClient().GetUser().Respect));

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, 7));
        }
    }
}