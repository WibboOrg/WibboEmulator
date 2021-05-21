using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class NotifTopInitComposer : ServerPacket
    {
        public NotifTopInitComposer(List<string> Messages)
         : base(19)
        {
            this.WriteInteger(Messages.Count);

            foreach (string Message in Messages)
            {
                this.WriteString(Message);
            }
        }
    }
}
