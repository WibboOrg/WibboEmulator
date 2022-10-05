namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;

internal class CreditBalanceComposer : ServerPacket
{
    public CreditBalanceComposer(int creditsBalance)
        : base(ServerPacketHeader.USER_CREDITS) => this.WriteString(creditsBalance + ".0");
}
