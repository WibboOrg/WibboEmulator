using WibboEmulator.Games.Clients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorJukebox : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights || Session == null || Item == null)
            {
                return;
            }
        }

        public override void OnTick(Item item)
        {
        }
    }
}
