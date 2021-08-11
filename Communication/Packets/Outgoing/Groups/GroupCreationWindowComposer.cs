using Butterfly.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class GroupCreationWindowComposer : ServerPacket
    {
        public GroupCreationWindowComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.GROUP_CREATE_OPTIONS)
        {
            this.WriteInteger(20);//Price

            this.WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData Room in Rooms)
            {
                this.WriteInteger(Room.Id);//Room Id
                this.WriteString(Room.Name);//Room Name
                this.WriteBoolean(false);//What?
            }

            this.WriteInteger(5);
            this.WriteInteger(5);
            this.WriteInteger(11);
            this.WriteInteger(4);

            this.WriteInteger(6);
            this.WriteInteger(11);
            this.WriteInteger(4);

            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);

            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);

            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);
        }
    }
}
