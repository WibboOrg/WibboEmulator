using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public abstract class FurniInteractor
    {
        public abstract void OnPlace(GameClient Session, Item Item);

        public abstract void OnRemove(GameClient Session, Item Item);

        public abstract void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights);
    }
}
