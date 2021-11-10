using Butterfly.Game.Rooms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserUpdateComposer : ServerPacket
    {
        public UserUpdateComposer(ICollection<RoomUser> RoomUsers)
            : base(ServerPacketHeader.UNIT_STATUS)
        {
            this.WriteInteger(RoomUsers.Count);
            foreach (RoomUser User in RoomUsers.ToList())
            {
                this.WriteInteger(User.VirtualId);
                this.WriteInteger(User.X);
                this.WriteInteger(User.Y);
                this.WriteString(User.Z.ToString("0.00"));
                this.WriteInteger(User.RotHead);
                this.WriteInteger(User.RotBody);

                StringBuilder StatusComposer = new StringBuilder();
                StatusComposer.Append("/");

                foreach (KeyValuePair<string, string> Status in User.Statusses.ToList())
                {
                    StatusComposer.Append(Status.Key);

                    if (!string.IsNullOrEmpty(Status.Value))
                    {
                        StatusComposer.Append(" ");
                        StatusComposer.Append(Status.Value);
                    }

                    StatusComposer.Append("/");
                }

                StatusComposer.Append("/");
                this.WriteString(StatusComposer.ToString());
            }
        }
    }
}
