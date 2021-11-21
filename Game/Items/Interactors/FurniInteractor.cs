using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
{
    public abstract class FurniInteractor
    {
        public abstract void OnPlace(Client Session, Item Item);

        public abstract void OnRemove(Client Session, Item Item);

        public abstract void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights);
    }
}
