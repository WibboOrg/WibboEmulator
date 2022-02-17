using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DanceEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            roomUserByHabbo.Unidle();
            int danceId = Packet.PopInt();
            if (danceId < 0 || danceId > 4 || !true && danceId > 1)
            {
                danceId = 0;
            }

            if (danceId > 0 && roomUserByHabbo.CarryItemID > 0)
            {
                roomUserByHabbo.CarryItem(0);
            }

            roomUserByHabbo.DanceId = danceId;
            room.SendPacket(new DanceComposer(roomUserByHabbo.VirtualId, danceId));
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_DANCE, 0);
        }
    }
}