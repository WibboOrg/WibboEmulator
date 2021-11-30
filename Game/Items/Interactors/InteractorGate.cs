using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorGate : FurniInteractor
    {
        public InteractorGate()
        {
        }

        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {

            if (!UserHasRights)
            {
                return;
            }

            int.TryParse(Item.ExtraData, out int currentMode);


            int newMode;
            if (currentMode == 0)
            {
                newMode = 1;
            }
            else
            {
                newMode = 0;
            }

            Item.ExtraData = newMode.ToString();
            Item.UpdateState();
            Item.GetRoom().GetGameMap().UpdateMapForItem(Item);
        }

        public override void OnTick(Item item)
        {
        }
    }
}
