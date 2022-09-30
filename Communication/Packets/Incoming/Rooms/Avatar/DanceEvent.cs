using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DanceEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

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