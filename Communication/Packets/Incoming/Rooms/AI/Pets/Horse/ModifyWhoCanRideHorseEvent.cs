using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModifyWhoCanRideHorseEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Room.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                return;
            }

            if (Pet.PetData == null || Pet.PetData.OwnerId != Session.GetUser().Id || Pet.PetData.Type != 13)
            {
                return;
            }

            if (Pet.PetData.AnyoneCanRide)
            {
                Pet.PetData.AnyoneCanRide = false;
            }
            else
            {
                Pet.PetData.AnyoneCanRide = true;
            }

            if (!Pet.PetData.AnyoneCanRide)
            {
                if (Pet.RidingHorse)
                {
                    Pet.RidingHorse = false;
                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
                    if (User != null)
                    {
                        if (Room.CheckRights(User.GetClient(), true))
                        {
                            User.RidingHorse = false;
                            User.HorseID = 0;
                            User.ApplyEffect(-1);
                            User.MoveTo(User.X + 1, User.Y + 1);
                        }
                    }
                }
            }


            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateAnyoneRide(dbClient, PetId, Pet.PetData.AnyoneCanRide);
            }

            Room.SendPacket(new PetInformationComposer(Pet.PetData, Pet.RidingHorse));
        }
    }
}
