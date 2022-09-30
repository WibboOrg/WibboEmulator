using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorGenericSwitch : FurniInteractor
    {
        private readonly int Modes;

        public InteractorGenericSwitch(int Modes)
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
            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
                if (roomUserByUserId != null)
                {
                    roomUserByUserId.CanWalk = true;
                }

                Item.InteractingUser = 0;
            }
            if (Item.InteractingUser2 != 0)
            {
                RoomUser roomUserByUserIdTwo = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser2);
                if (roomUserByUserIdTwo != null)
                {
                    roomUserByUserIdTwo.CanWalk = true;
                }

                Item.InteractingUser2 = 0;
            }

            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
            {
                if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                {
                    Item.ExtraData = "0;" + Item.GroupId;
                }
                else
                {
                    Item.ExtraData = "0";
                }
            }
        }

        public override void OnRemove(Client Session, Item Item)
        {
            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
                if (roomUserByUserId != null)
                {
                    roomUserByUserId.CanWalk = true;
                }

                Item.InteractingUser = 0;
            }
            if (Item.InteractingUser2 != 0)
            {
                RoomUser roomUserByUserIdTwo = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser2);
                if (roomUserByUserIdTwo != null)
                {
                    roomUserByUserIdTwo.CanWalk = true;
                }

                Item.InteractingUser2 = 0;
            }
            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
            {
                if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                {
                    Item.ExtraData = "0;" + Item.GroupId;
                }
                else
                {
                    Item.ExtraData = "0";
                }
            }
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session != null)
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH, 0);
            }

            if (!UserHasRights || this.Modes == 0)
            {
                return;
            }

            int state;
            if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
            {
                int.TryParse(Item.ExtraData.Split(';')[0], out state);
            }
            else
            {
                int.TryParse(Item.ExtraData, out state);
            }

            int newState;

            if (Reverse)
                newState = (state > 0 ? state - 1 : this.Modes);
            else
                newState = (state < this.Modes ? state + 1 : 0);

            if (Session != null && Session.GetUser() != null && Session.GetUser().ForceUse > -1)
            {
                newState = (Session.GetUser().ForceUse <= this.Modes) ? Session.GetUser().ForceUse : 0;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
            {
                Item.ExtraData = newState.ToString() + ";" + Item.GroupId;
            }
            else
            {
                Item.ExtraData = newState.ToString();
            }

            Item.UpdateState();

            if (Item.GetBaseItem().AdjustableHeights.Count > 1)
            {
                if (Session == null)
                {
                    return;
                }

                if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                    return;

                RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                if (roomUserByUserId != null)
                {
                    Item.GetRoom().GetRoomUserManager().UpdateUserStatus(roomUserByUserId, false);
                }
            }
        }

        public override void OnTick(Item item)
        {
        }
    }
}
