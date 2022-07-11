using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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