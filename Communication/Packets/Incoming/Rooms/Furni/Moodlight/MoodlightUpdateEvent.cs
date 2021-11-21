using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class MoodlightUpdateEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null || !room.CheckRights(Session, true) || room.MoodlightData == null)
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
            {
                return;
            }

            int Preset = Packet.PopInt();            int num = Packet.PopInt();            string Color = Packet.PopString();            int Intensity = Packet.PopInt();            bool BgOnly = false;            if (num >= 2)
            {
                BgOnly = true;
            }

            room.MoodlightData.Enabled = true;            room.MoodlightData.CurrentPreset = Preset;            room.MoodlightData.UpdatePreset(Preset, Color, Intensity, BgOnly);            roomItem.ExtraData = room.MoodlightData.GenerateExtraData();            roomItem.UpdateState();
        }
    }
}
