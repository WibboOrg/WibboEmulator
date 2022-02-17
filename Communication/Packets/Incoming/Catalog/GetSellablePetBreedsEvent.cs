using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetSellablePetBreedsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            ItemData Item = ButterflyEnvironment.GetGame().GetItemManager().GetItemByName(Type);
            if (Item == null)
            {
                return;
            }

            int PetId = Item.SpriteId;

            Session.SendPacket(new SellablePetBreedsComposer(Type, PetId, ButterflyEnvironment.GetGame().GetCatalog().GetRacesForRaceId(PetId)));
        }
    }
}
