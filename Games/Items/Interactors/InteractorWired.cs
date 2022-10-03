﻿using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorWired : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Item == null || !UserHasRights)
            {
                return;
            }

            if (Item.WiredHandler != null)
            {
                Item.WiredHandler.OnTrigger(Session);
            }

            Item.ExtraData = "1";
            Item.UpdateState();
            Item.ReqUpdate(4);
        }

        public override void OnTick(Item item)
        {
            item.ExtraData = "0";
            item.UpdateState();
        }
    }
}