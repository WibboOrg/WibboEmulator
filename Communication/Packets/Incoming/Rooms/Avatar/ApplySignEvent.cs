using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ApplySignEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            roomUserByUserId.Unidle();

            int num = Packet.PopInt();
            if (roomUserByUserId.ContainStatus("sign"))
            {
                roomUserByUserId.RemoveStatus("sign");
            }

            roomUserByUserId.SetStatus("sign", Convert.ToString(num));
            roomUserByUserId.UpdateNeeded = true;
        }
    }
}
