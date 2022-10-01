using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorGate : FurniInteractor
    {
        public InteractorGate()
        {
        }

        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
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
            Item.GetRoom()?.GetGameMap().UpdateMapForItem(Item);
        }

        public override void OnTick(Item item)
        {
        }
    }
}
