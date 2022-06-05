using Wibbo.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetPetTrainingPanelEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().InRoom)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Session.GetUser().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                return;
            }

            //Continue as a regular pet..
            if (Pet.RoomId != Session.GetUser().CurrentRoomId || Pet.PetData == null)
            {
                return;
            }

            Session.SendPacket(new PetTrainingPanelComposer(Pet.PetData));
        }
    }
}