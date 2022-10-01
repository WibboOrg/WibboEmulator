using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
