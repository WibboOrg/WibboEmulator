using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ActionEvent : IPacketEvent
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
            int i = Packet.PopInt();
            roomUserByUserId.DanceId = 0;

            room.SendPacket(new ActionComposer(roomUserByUserId.VirtualId, i));

            if (i == 5)
            {
                roomUserByUserId.IsAsleep = true;
                room.SendPacket(new SleepComposer(roomUserByUserId.VirtualId, true));
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_WAVE, 0);
        }
    }
}
