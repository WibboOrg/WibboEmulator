using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorJukebox : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights || Session == null || Item == null)
            {
                return;
            }
        }
    }
}
