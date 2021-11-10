using Butterfly.Game.Pets;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Outgoing.Pets
{
    internal class RespectPetNotificationComposer : ServerPacket
    {
        public RespectPetNotificationComposer(Pet Pet)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            //TODO: Structure
            this.WriteInteger(Pet.VirtualId);
            this.WriteInteger(Pet.VirtualId);
            this.WriteInteger(Pet.PetId);//Pet Id, 100%
            this.WriteString(Pet.Name);
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteString(Pet.Color);
            this.WriteInteger(0);
            this.WriteInteger(0);//Count - 3 ints.
            this.WriteInteger(1);
        }

        public RespectPetNotificationComposer(Habbo Habbo, RoomUser User)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            //TODO: Structure
            this.WriteInteger(User.VirtualId);
            this.WriteInteger(User.VirtualId);
            this.WriteInteger(Habbo.Id);//Pet Id, 100%
            this.WriteString(Habbo.Username);
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteString("FFFFFF");//Yeah..
            this.WriteInteger(0);
            this.WriteInteger(0);//Count - 3 ints.
            this.WriteInteger(1);
        }
    }
}
