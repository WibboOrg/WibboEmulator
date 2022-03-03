using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorChangeBackgrounds : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Session.GetUser() == null || (Item == null || !UserHasRights) || Request != 0)
            {
                return;
            }

            if (Item.ExtraData.StartsWith("on"))
            {
                Item.ExtraData = Item.ExtraData.Replace("on", "off");
            }
            else if (Item.ExtraData.StartsWith("off"))
            {
                Item.ExtraData = Item.ExtraData.Replace("off", "on");
            }

            Item.UpdateState();
        }

        public override void OnTick(Item item)
        {
        }
    }
}
