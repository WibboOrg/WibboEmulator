using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorScoreCounter : FurniInteractor
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

            int num = 0;
            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    num = int.Parse(Item.ExtraData);
                }
                catch
                {
                }
            }

            if (Request == 1)
            {
                num++;
            }
            else if (Request == 2)
            {
                num--;
            }
            else if (Request == 3)
            {
                num = 0;
            }

            Item.ExtraData = num.ToString();
            Item.UpdateState(false, true);
        }
    }
}
