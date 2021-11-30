using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorHabboWheel : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "-1";
            Item.ReqUpdate(10);
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "-1";
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
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
            item.ExtraData = ButterflyEnvironment.GetRandomNumber(1, 10).ToString();
            item.UpdateState();
        }
    }
}
