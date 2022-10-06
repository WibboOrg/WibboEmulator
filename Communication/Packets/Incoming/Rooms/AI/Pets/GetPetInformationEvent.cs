namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;

internal class GetPetInformationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null || session.GetUser().CurrentRoom == null)
        {
            return;
        }

        var PetId = packet.PopInt();

        if (!session.GetUser().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out var Pet))
        {
            //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
            var User = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(PetId);
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
            session.SendPacket(new PetInformationComposer(User.GetClient().GetUser()));
            return;
        }

        //Continue as a regular pet..
        if (Pet.RoomId != session.GetUser().CurrentRoomId || Pet.PetData == null)
        {
            return;
        }

        session.SendPacket(new PetInformationComposer(Pet.PetData, Pet.RidingHorse));
    }
}