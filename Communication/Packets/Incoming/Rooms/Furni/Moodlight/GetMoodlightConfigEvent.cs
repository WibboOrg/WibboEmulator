using Wibbo.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetMoodlightConfigEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
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