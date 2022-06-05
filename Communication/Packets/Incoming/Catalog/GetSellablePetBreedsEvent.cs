using Wibbo.Communication.Packets.Outgoing.Catalog;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetSellablePetBreedsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            ItemData Item = WibboEnvironment.GetGame().GetItemManager().GetItemByName(Type);
            if (Item == null)
            {
                return;
            }

            int PetId = Item.SpriteId;

            Session.SendPacket(new SellablePetBreedsComposer(Type, PetId, WibboEnvironment.GetGame().GetCatalog().GetRacesForRaceId(PetId)));
        }
    }
}
