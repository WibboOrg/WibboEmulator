using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ToggleMoodlightEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true) || room.MoodlightData == null)
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
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

            roomItem.ExtraData = room.MoodlightData.GenerateExtraData();
            roomItem.UpdateState();
        }
    }
}
