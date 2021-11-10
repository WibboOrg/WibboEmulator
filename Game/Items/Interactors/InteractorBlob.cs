﻿using Butterfly.Game.GameClients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorBlob : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "1";
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "1";
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            if (Item.ExtraData == "0")
            {
                return;
            }

            Item.ExtraData = "0";
            Item.UpdateState();
        }
    }
}