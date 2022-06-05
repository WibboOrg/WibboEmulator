using Wibbo.Communication.Packets.Outgoing.Rooms.Avatar;
using Wibbo.Game.Clients;
using Wibbo.Game.Quests;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class DanceEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
            int danceId = Packet.PopInt();
            if (danceId < 0 || danceId > 4 || !true && danceId > 1)
            {
                danceId = 0;
            }

            if (danceId > 0 && roomUserByUserId.CarryItemID > 0)
            {
                roomUserByUserId.CarryItem(0);
            }

            roomUserByUserId.DanceId = danceId;
            room.SendPacket(new DanceComposer(roomUserByUserId.VirtualId, danceId));
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_DANCE, 0);
        }
    }
}