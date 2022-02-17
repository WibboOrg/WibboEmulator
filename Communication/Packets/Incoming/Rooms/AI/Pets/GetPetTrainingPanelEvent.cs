using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetPetTrainingPanelEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                return;
            }

            //Continue as a regular pet..
            if (Pet.RoomId != Session.GetHabbo().CurrentRoomId || Pet.PetData == null)
            {
                return;
            }

            Session.SendPacket(new PetTrainingPanelComposer(Pet.PetData));
        }
    }
}