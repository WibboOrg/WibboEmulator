using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Moodlight;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetMoodlightConfigEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            if (room.MoodlightData == null || room.MoodlightData.Presets == null)
            {
                return;
            }

            ServerPacket Response = new ServerPacket(ServerPacketHeader.ITEM_DIMMER_SETTINGS);
            Response.WriteInteger(room.MoodlightData.Presets.Count);
            Response.WriteInteger(room.MoodlightData.CurrentPreset);

            int i = 0;
            foreach (MoodlightPreset moodlightPreset in room.MoodlightData.Presets)
            {
                i++;
                Response.WriteInteger(i);
                Response.WriteInteger(int.Parse(ButterflyEnvironment.BoolToEnum(moodlightPreset.BackgroundOnly)) + 1);
                Response.WriteString(moodlightPreset.ColorCode);
                Response.WriteInteger(moodlightPreset.ColorIntensity);
            }
            Session.SendPacket(Response);
        }
    }
}