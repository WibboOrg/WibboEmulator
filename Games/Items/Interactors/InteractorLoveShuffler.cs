using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorLoveShuffler : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights || !(Item.ExtraData != "0"))
            {
                return;
            }

            Item.ExtraData = "0";
            Item.UpdateState(false, true);
            Item.ReqUpdate(10);
        }

        public override void OnTick(Item item)
        {
            if (item.ExtraData == "0")
            {
                item.ExtraData = WibboEnvironment.GetRandomNumber(1, 4).ToString();
                item.ReqUpdate(20);
            }
            else if (item.ExtraData != "-1")
            {
                item.ExtraData = "-1";
            }

            item.UpdateState(false, true);
        }
    }
}
