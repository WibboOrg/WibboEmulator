using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class BotChooseComposer : ServerPacket
    {
        public BotChooseComposer(List<string[]> ChooseList)
          : base(ServerPacketHeader.BOT_CHOOSE)
        {
            this.WriteInteger(ChooseList.Count);

            foreach (string[] Choose in ChooseList)
            {
                this.WriteString(Choose[0]); //Username
                this.WriteString(Choose[1]); //Code
                this.WriteString(Choose[2]); //Message
                this.WriteString(Choose[3]); //Look
            }
        }
    }
}
