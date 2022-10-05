using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorAlert : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnRemove(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights || !(Item.ExtraData == "0"))
            {
                return;
            }

            Item.ExtraData = "1";
            Item.UpdateState(false, true);
            Item.ReqUpdate(4);
        }

        public override void OnTick(Item item)
        {
            if (!(item.ExtraData == "1"))
            {
                return;
            }

            item.ExtraData = "0";
            item.UpdateState(false, true);
        }
    }
}
