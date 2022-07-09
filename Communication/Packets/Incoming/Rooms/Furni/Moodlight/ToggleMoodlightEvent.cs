using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class ToggleMoodlightEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);            if (room == null || !room.CheckRights(Session, true) || room.MoodlightData == null)
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
            {
                return;
            }

            if (room.MoodlightData.Enabled)
            {
                room.MoodlightData.Disable();
            }
            else
            {
                room.MoodlightData.Enable();
            }

            roomItem.ExtraData = room.MoodlightData.GenerateExtraData();            roomItem.UpdateState();
        }
    }
}
