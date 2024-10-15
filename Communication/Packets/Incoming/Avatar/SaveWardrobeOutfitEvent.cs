namespace WibboEmulator.Communication.Packets.Incoming.Avatar;
using WibboEmulator.Games.GameClients;

internal sealed class SaveWardrobeOutfitEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var slotId = packet.PopInt();
        var look = packet.PopString();
        var gender = packet.PopString(1);

        Session.User.WardrobeComponent.AddWardrobe(look, gender, slotId);
    }
}
