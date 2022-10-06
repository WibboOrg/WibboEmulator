namespace WibboEmulator.Communication.Packets.Incoming.Avatar;
using WibboEmulator.Games.GameClients;

internal class SaveWardrobeOutfitEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var slotId = Packet.PopInt();
        var look = Packet.PopString();
        var gender = Packet.PopString();

        session.GetUser().GetWardrobeComponent().AddWardobe(look, gender, slotId);
    }
}
