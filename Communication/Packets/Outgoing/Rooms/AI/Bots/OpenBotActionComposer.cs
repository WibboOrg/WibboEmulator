namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Bots;
using WibboEmulator.Games.Rooms;

internal sealed class OpenBotActionComposer : ServerPacket
{
    public OpenBotActionComposer(RoomUser botUser, int actionId, string botSpeech)
        : base(ServerPacketHeader.BOT_COMMAND_CONFIGURATION)
    {
        this.WriteInteger(botUser.BotData.Id);
        this.WriteInteger(actionId);
        if (actionId == 2)
        {
            this.WriteString(botSpeech);
        }
        else if (actionId == 5)
        {
            this.WriteString(botUser.BotData.Name);
        }
    }
}
