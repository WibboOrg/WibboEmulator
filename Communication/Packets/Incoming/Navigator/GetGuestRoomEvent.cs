using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Clients;using Butterfly.Game.Rooms;
namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class GetGuestRoomEvent : IPacketEvent    {        public void Parse(Client Session, ClientPacket Packet)        {
            int roomID = Packet.PopInt();

            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
            {
                return;
            }

            bool isLoading = Packet.PopInt() == 1;
            bool checkEntry = Packet.PopInt() == 1;

            Session.SendPacket(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));        }    }}