using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorBanzaiScoreCounter : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            if (Item.Team == TeamType.none)
            {
                return;
            }

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.Team].ToString();
            Item.UpdateState(false, true);
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

            Item.GetRoom().GetGameManager().Points[(int)Item.Team] = 0;
            Item.ExtraData = "0";
            Item.UpdateState();
        }
    }
}
