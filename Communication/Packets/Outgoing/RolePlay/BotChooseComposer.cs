namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay;

internal class BotChooseComposer : ServerPacket
{
    public BotChooseComposer(List<string[]> chooseList)
      : base(ServerPacketHeader.BOT_CHOOSE)
    {
        this.WriteInteger(chooseList.Count);

        foreach (var choose in chooseList)
        {
            this.WriteString(choose[0]); //Username
            this.WriteString(choose[1]); //Code
            this.WriteString(choose[2]); //Message
            this.WriteString(choose[3]); //Look
        }
    }
}
