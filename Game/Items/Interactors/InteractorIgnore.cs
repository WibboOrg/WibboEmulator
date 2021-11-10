using Butterfly.Game.GameClients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorIgnore : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
        }
    }
}
