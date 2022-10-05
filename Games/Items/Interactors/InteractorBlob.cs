using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorBlob : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item) => Item.ExtraData = "1";

        public override void OnRemove(GameClient Session, Item Item) => Item.ExtraData = "1";

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
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
