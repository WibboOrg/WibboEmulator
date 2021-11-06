using Butterfly.HabboHotel.Moderation;
using Butterfly.Utilities;
using System;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketResponseMessageComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseMessageComposer(int Result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            WriteInteger(Result);
            WriteString("");
        }
    }
}
