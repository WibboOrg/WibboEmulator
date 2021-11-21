using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorAlert : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights || !(Item.ExtraData == "0"))
            {
                return;
            }

            Item.ExtraData = "1";
            Item.UpdateState(false, true);
            Item.ReqUpdate(4);
        }
    }
}
