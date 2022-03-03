using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || Session.GetUser().DailyRespectPoints <= 0)
            {
                return;
            }

            RoomUser roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(Packet.PopInt());
            if (roomUserByUserIdTarget == null || roomUserByUserIdTarget.GetClient().GetUser().Id == Session.GetUser().Id || roomUserByUserIdTarget.IsBot)
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT, 0);
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserIdTarget.GetClient(), "ACH_RespectEarned", 1);
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            Session.GetUser().DailyRespectPoints--;
            roomUserByUserIdTarget.GetClient().GetUser().Respect++;

            room.SendPacket(new RespectNotificationComposer(roomUserByUserIdTarget.GetClient().GetUser().Id, roomUserByUserIdTarget.GetClient().GetUser().Respect));

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, 7));
        }
    }
}