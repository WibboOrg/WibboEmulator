using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Pets;
using Butterfly.Game.Rooms;
using System.Drawing;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class PickUpPetEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            if (Session == null || Session.GetUser() == null || Session.GetUser().GetInventoryComponent() == null)
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
                //Check kick rights, just because it seems most appropriate.
                if ((!Room.CheckRights(Session) && Room.RoomData.WhoCanKick != 2 && Room.RoomData.Group == null) || (Room.RoomData.Group != null && !Room.CheckRights(Session)))
                {
                    return;
                }

                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser TargetUser = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(PetId);
                if (TargetUser == null)
                {
                    return;
                }

                //Check some values first, please!
                if (TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
                {
                    return;
                }

                TargetUser.transformation = false;

                //Quickly remove the old user instance.
                Room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

                //Add the new one, they won't even notice a thing!!11 8-)
                Room.SendPacket(new UsersComposer(TargetUser));
                return;
            }

            if (Session.GetUser().Id != Pet.PetData.OwnerId && !Room.CheckRights(Session, true))
            {
                return;
            }

            if (Pet.RidingHorse)
            {
                RoomUser UserRiding = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
                if (UserRiding != null)
                {
                    UserRiding.RidingHorse = false;
                    UserRiding.ApplyEffect(-1);
                    UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
                }
                else
                {
                    Pet.RidingHorse = false;
                }
            }

            Pet pet = Pet.PetData;

            pet.RoomId = 0;
            pet.PlacedInRoom = false;

            pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateRoomId(dbClient, pet.PetId, 0);
            }

            if (pet.OwnerId != Session.GetUser().Id)
            {
                Client Target = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(pet.OwnerId);
                if (Target != null)
                {
                    Target.GetUser().GetInventoryComponent().TryAddPet(Pet.PetData);
                    Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);

                    Target.SendPacket(new PetInventoryComposer(Target.GetUser().GetInventoryComponent().GetPets()));
                    return;
                }
            }
            else
            {
                Session.GetUser().GetInventoryComponent().TryAddPet(Pet.PetData);
                Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);
                Session.SendPacket(new PetInventoryComposer(Session.GetUser().GetInventoryComponent().GetPets()));
            }
        }
    }
}
