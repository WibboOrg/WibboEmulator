namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;

using WibboEmulator.Games.GameClients;

internal class CheckPetNameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var PetName = Packet.PopString();

        session.SendPacket(new CheckPetNameComposer(PetName));
    }
}