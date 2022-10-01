using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetMoodlightConfigEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
            {
                return;
            }

            if (room.MoodlightData == null || room.MoodlightData.Presets == null)
            {
                return;
            }

            Session.SendPacket(new MoodlightConfigComposer(room.MoodlightData));
        }
    }
}