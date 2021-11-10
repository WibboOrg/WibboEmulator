using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots
{
    internal class OpenBotActionComposer : ServerPacket
    {
        public OpenBotActionComposer(RoomUser BotUser, int ActionId, string BotSpeech)
            : base(ServerPacketHeader.BOT_COMMAND_CONFIGURATION)
        {
            this.WriteInteger(BotUser.BotData.Id);
            this.WriteInteger(ActionId);
            if (ActionId == 2)
            {
                this.WriteString(BotSpeech);
            }
            else if (ActionId == 5)
            {
                this.WriteString(BotUser.BotData.Name);
            }
        }
    }
}
