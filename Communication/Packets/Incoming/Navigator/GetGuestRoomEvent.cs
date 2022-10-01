using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetGuestRoomEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();

            RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
            {
                return;
            }

            bool isLoading = Packet.PopInt() == 1;
            bool checkEntry = Packet.PopInt() == 1;

            Session.SendPacket(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));
        }
    }
}
