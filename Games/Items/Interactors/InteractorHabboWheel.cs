using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorHabboWheel : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnRemove(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights || !(Item.ExtraData != "-1"))
            {
                return;
            }

            Item.ExtraData = "-1";
            Item.UpdateState();

            Item.ReqUpdate(10);
        }

        public override void OnTick(Item item)
        {
            item.ExtraData = WibboEnvironment.GetRandomNumber(1, 10).ToString();
            item.UpdateState();
        }
    }
}
