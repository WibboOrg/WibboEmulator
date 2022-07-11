using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorSwitch : FurniInteractor
    {
        private readonly int Modes;

        public InteractorSwitch(int Modes)
        {
            this.Modes = Modes - 1;
            if (this.Modes >= 0)
            {
                return;
            }

            this.Modes = 0;
        }

        public override void OnPlace(Client Session, Item Item)
        {
            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
            {
                Item.ExtraData = "0";
            }
        }

        public override void OnRemove(Client Session, Item Item)
        {
            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
            {
                Item.ExtraData = "0";
            }
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session != null)
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH, 0);
            }

            if (this.Modes == 0)
            {
                return;
            }

            RoomUser roomUser = null;
            if (Session != null)
            {
                roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            }

            if (roomUser == null)
            {
                return;
            }

            if (!Gamemap.TilesTouching(Item.X, Item.Y, roomUser.X, roomUser.Y))
            {
                return;
            }

            int.TryParse(Item.ExtraData, out int state);

            if(Reverse)
                Item.ExtraData = (state > 0 ? state - 1 : this.Modes).ToString();
            else
                Item.ExtraData = (state < this.Modes ? state + 1 : 0).ToString();

            Item.UpdateState();
        }

        public override void OnTick(Item item)
        {
        }
    }
}
