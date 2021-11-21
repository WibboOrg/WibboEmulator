using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
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

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            if (Item.ExtraData == "0")
            {
                return;
            }

            Item.ExtraData = "0";
            Item.UpdateState();
        }
    }
}
