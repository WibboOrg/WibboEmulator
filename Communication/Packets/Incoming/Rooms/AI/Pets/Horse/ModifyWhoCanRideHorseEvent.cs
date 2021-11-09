using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModifyWhoCanRideHorseEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Room.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                return;
            }

            if (Pet.PetData == null || Pet.PetData.OwnerId != Session.GetHabbo().Id || Pet.PetData.Type != 13)
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
                PetDao.UpdateAnyoneRide(dbClient, PetId, Pet.PetData.AnyoneCanRide);
            }

            Room.SendPacket(new PetInformationComposer(Pet.PetData, Pet.RidingHorse));
        }
    }
}
