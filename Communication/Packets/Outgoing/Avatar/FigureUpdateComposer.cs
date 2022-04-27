using Butterfly.Game.Users.Wardrobes;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Avatar
{
    internal class FigureUpdateComposer : ServerPacket
    {
        public FigureUpdateComposer(string figure, string gender)
            : base(ServerPacketHeader.USER_FIGURE)
        {
            this.WriteString(figure);
            this.WriteString(gender);
        }
    }
}