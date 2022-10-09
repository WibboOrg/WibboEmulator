namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users;

internal class PetInformationComposer : ServerPacket
{
    public PetInformationComposer(Pet pet, bool isRide = false)
        : base(ServerPacketHeader.PET_INFO)
    {
        this.WriteInteger(pet.PetId);
        this.WriteString(pet.Name);
        this.WriteInteger(pet.Level);
        this.WriteInteger(Pet.MaxLevel);
        this.WriteInteger(pet.Expirience);
        this.WriteInteger(pet.ExpirienceGoal);
        this.WriteInteger(pet.Energy);
        this.WriteInteger(Pet.MaxEnergy);
        this.WriteInteger(pet.Nutrition);
        this.WriteInteger(Pet.MaxNutrition);
        this.WriteInteger(pet.Respect);
        this.WriteInteger(pet.OwnerId);
        this.WriteInteger(pet.Age);
        this.WriteString(pet.OwnerName);
        this.WriteInteger(1);//3 on hab
        this.WriteBoolean(pet.Saddle > 0);
        this.WriteBoolean(isRide);
        this.WriteInteger(0);//5 on hab
        this.WriteInteger(pet.AnyoneCanRide ? 1 : 0); // Anyone can ride horse
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
