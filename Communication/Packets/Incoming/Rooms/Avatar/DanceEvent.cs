using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DanceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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
            int i = Packet.PopInt();
            if (i < 0 || i > 4 || !true && i > 1)
            {
                i = 0;
            }

            if (i > 0 && roomUserByHabbo.CarryItemID > 0)
            {
                roomUserByHabbo.CarryItem(0);
            }

            roomUserByHabbo.DanceId = i;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_DANCE);
            Message.WriteInteger(roomUserByHabbo.VirtualId);
            Message.WriteInteger(i);
            room.SendPacket(Message);
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_DANCE, 0);
        }
    }
}