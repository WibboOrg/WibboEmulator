namespace WibboEmulator.Communication.Packets.Outgoing.Pets;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users;

internal sealed class RespectPetNotificationComposer : ServerPacket
{
    public RespectPetNotificationComposer(Pet pet)
        : base(ServerPacketHeader.PET_RESPECTED)
    {
        //TODO: Structure
        this.WriteInteger(pet.VirtualId);
        this.WriteInteger(pet.VirtualId);
        this.WriteInteger(pet.PetId);//Pet Id, 100%
        this.WriteString(pet.Name);
        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteString(pet.Color);
        this.WriteInteger(0);
        this.WriteInteger(0);//Count - 3 ints.
        this.WriteInteger(1);
    }

    public RespectPetNotificationComposer(User user, RoomUser rooomUser)
        : base(ServerPacketHeader.PET_RESPECTED)
    {
        //TODO: Structure
        this.WriteInteger(rooomUser.VirtualId);
        this.WriteInteger(rooomUser.VirtualId);
        this.WriteInteger(user.Id);//Pet Id, 100%
        this.WriteString(user.Username);
        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteString("FFFFFF");//Yeah..
        this.WriteInteger(0);
        this.WriteInteger(0);//Count - 3 ints.
        this.WriteInteger(1);
    }
}
