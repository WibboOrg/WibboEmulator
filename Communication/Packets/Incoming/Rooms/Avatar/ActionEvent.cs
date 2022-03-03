using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;

using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ActionEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            roomUserByUserId.Unidle();
            int i = Packet.PopInt();
            roomUserByUserId.DanceId = 0;

            room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, i));

            if (i == 5)
            {
                roomUserByUserId.IsAsleep = true;
                room.SendPacket(new SleepComposer(roomUserByUserId.VirtualId, true));
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_WAVE, 0);
        }
    }
}
