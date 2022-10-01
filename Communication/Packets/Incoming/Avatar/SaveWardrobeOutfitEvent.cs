using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int slotId = Packet.PopInt();
            string look = Packet.PopString();
            string gender = Packet.PopString();

            Session.GetUser().GetWardrobeComponent().AddWardobe(look, gender, slotId);
        }
    }
}
