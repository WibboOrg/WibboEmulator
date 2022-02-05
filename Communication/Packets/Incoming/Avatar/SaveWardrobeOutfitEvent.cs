using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int slotId = Packet.PopInt();
            string look = Packet.PopString();
            string gender = Packet.PopString();

            Session.GetHabbo().GetWardrobeComponent().AddWardobe(look, gender, slotId);
        }
    }
}
