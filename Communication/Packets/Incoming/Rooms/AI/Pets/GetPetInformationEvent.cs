using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetPetInformationEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null || Session.GetUser().CurrentRoom == null)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Session.GetUser().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser User = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(PetId);
                if (User == null)
                {
                    return;
                }

                //Check some values first, please!
                if (User.GetClient() == null || User.GetClient().GetUser() == null)
                {
                    return;
                }

                //And boom! Let us send the information composer 8-).
                Session.SendPacket(new PetInformationComposer(User.GetClient().GetUser()));
                return;
            }

            //Continue as a regular pet..
            if (Pet.RoomId != Session.GetUser().CurrentRoomId || Pet.PetData == null)
            {
                return;
            }

            Session.SendPacket(new PetInformationComposer(Pet.PetData, Pet.RidingHorse));
        }
    }
}