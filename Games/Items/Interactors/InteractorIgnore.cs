using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorIgnore : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
        }

        public override void OnTick(Item item)
        {
        }
    }
}
