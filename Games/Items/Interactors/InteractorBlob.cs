using WibboEmulator.Game.Clients;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorBlob : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "1";
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "1";
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights || Item.ExtraData == "0")
            {
                return;
            }

            Item.ExtraData = "0";
            Item.UpdateState();
        }

        public override void OnTick(Item item)
        {
        }
    }
}
