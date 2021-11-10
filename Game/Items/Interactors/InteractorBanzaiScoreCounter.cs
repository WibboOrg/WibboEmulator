using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorBanzaiScoreCounter : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            if (Item.Team == Team.none)
            {
                return;
            }

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.Team].ToString();
            Item.UpdateState(false, true);
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
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
