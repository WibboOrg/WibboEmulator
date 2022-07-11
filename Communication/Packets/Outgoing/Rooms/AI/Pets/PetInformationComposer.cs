using WibboEmulator.Game.Pets;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Users;

namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class PetInformationComposer : ServerPacket
    {
        public PetInformationComposer(Pet Pet, bool IsRide = false)
            : base(ServerPacketHeader.PET_INFO)
        {

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Pet.RoomId, out Room Room))
            {
                return;
            }

            this.WriteInteger(Pet.PetId);
            this.WriteString(Pet.Name);
            this.WriteInteger(Pet.Level);
            this.WriteInteger(Pet.MaxLevel);
            this.WriteInteger(Pet.Expirience);
            this.WriteInteger(Pet.ExpirienceGoal);
            this.WriteInteger(Pet.Energy);
            this.WriteInteger(Pet.MaxEnergy);
            this.WriteInteger(Pet.Nutrition);
            this.WriteInteger(Pet.MaxNutrition);
            this.WriteInteger(Pet.Respect);
            this.WriteInteger(Pet.OwnerId);
            this.WriteInteger(Pet.Age);
            this.WriteString(Pet.OwnerName);
            this.WriteInteger(1);//3 on hab
            this.WriteBoolean(Pet.Saddle > 0);
            this.WriteBoolean(IsRide);
            this.WriteInteger(0);//5 on hab
            this.WriteInteger(Pet.AnyoneCanRide ? 1 : 0); // Anyone can ride horse
            this.WriteInteger(0);
            this.WriteInteger(0);//512 on hab
            this.WriteInteger(0);//1536
            this.WriteInteger(0);//2560
            this.WriteInteger(0);//3584
            this.WriteInteger(0);
            this.WriteString("");
            this.WriteBoolean(false);
            this.WriteInteger(-1);//255 on hab
            this.WriteInteger(-1);
            this.WriteInteger(-1);
            this.WriteBoolean(false);
        }

        public PetInformationComposer(User user)
            : base(ServerPacketHeader.PET_INFO)
        {
            this.WriteInteger(user.Id);
            this.WriteString(user.Username);
            this.WriteInteger(user.Rank);
            this.WriteInteger(10);
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(100);
            this.WriteInteger(100);
            this.WriteInteger(100);
            this.WriteInteger(100);
            this.WriteInteger(user.Respect);
            this.WriteInteger(user.Id);
            this.WriteInteger(0);//account created
            this.WriteString(user.Username);
            this.WriteInteger(1);//3 on hab
            this.WriteBoolean(false);
            this.WriteBoolean(false);
            this.WriteInteger(0);//5 on hab
            this.WriteInteger(0); // Anyone can ride horse
            this.WriteInteger(0);
            this.WriteInteger(0);//512 on hab
            this.WriteInteger(0);//1536
            this.WriteInteger(0);//2560
            this.WriteInteger(0);//3584
            this.WriteInteger(0);
            this.WriteString("");
            this.WriteBoolean(false);
            this.WriteInteger(-1);//255 on hab
            this.WriteInteger(-1);
            this.WriteInteger(-1);
            this.WriteBoolean(false);
        }
    }
}
