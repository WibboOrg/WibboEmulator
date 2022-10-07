namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

public class InteractorGenericSwitch : FurniInteractor
{
    private readonly int _modes;

    public InteractorGenericSwitch(int modes)
    {
        this._modes = modes - 1;
        if (this._modes >= 0)
        {
            return;
        }

        this._modes = 0;
    }

    public override void OnPlace(GameClient session, Item item)
    {
        if (item.InteractingUser != 0)
        {
            var roomUserByUserId = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.CanWalk = true;
            }

            item.InteractingUser = 0;
        }
        if (item.InteractingUser2 != 0)
        {
            var roomUserByUserIdTwo = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser2);
            if (roomUserByUserIdTwo != null)
            {
                roomUserByUserIdTwo.CanWalk = true;
            }

            item.InteractingUser2 = 0;
        }

        if (string.IsNullOrEmpty(item.ExtraData) && this._modes > 0)
        {
            if (item.GetBaseItem().InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
            {
                item.ExtraData = "0;" + item.GroupId;
            }
            else
            {
                item.ExtraData = "0";
            }
        }
    }

    public override void OnRemove(GameClient session, Item item)
    {
        if (item.InteractingUser != 0)
        {
            var roomUserByUserId = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.CanWalk = true;
            }

            item.InteractingUser = 0;
        }
        if (item.InteractingUser2 != 0)
        {
            var roomUserByUserIdTwo = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser2);
            if (roomUserByUserIdTwo != null)
            {
                roomUserByUserIdTwo.CanWalk = true;
            }

            item.InteractingUser2 = 0;
        }
        if (string.IsNullOrEmpty(item.ExtraData) && this._modes > 0)
        {
            if (item.GetBaseItem().InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
            {
                item.ExtraData = "0;" + item.GroupId;
            }
            else
            {
                item.ExtraData = "0";
            }
        }
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session != null)
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FURNI_SWITCH, 0);
        }

        if (!userHasRights || this._modes == 0)
        {
            return;
        }

        int state;
        if (item.GetBaseItem().InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
        {
            _ = int.TryParse(item.ExtraData.Split(';')[0], out state);
        }
        else
        {
            _ = int.TryParse(item.ExtraData, out state);
        }

        int newState;

        if (reverse)
        {
            newState = state > 0 ? state - 1 : this._modes;
        }
        else
        {
            newState = state < this._modes ? state + 1 : 0;
        }

        if (session != null && session.GetUser() != null && session.GetUser().ForceUse > -1)
        {
            newState = (session.GetUser().ForceUse <= this._modes) ? session.GetUser().ForceUse : 0;
        }

        if (item.GetBaseItem().InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
        {
            item.ExtraData = newState.ToString() + ";" + item.GroupId;
        }
        else
        {
            item.ExtraData = newState.ToString();
        }

        item.UpdateState();

        if (item.GetBaseItem().AdjustableHeights.Count > 1)
        {
            if (session == null)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
            {
                return;
            }

            var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (roomUserByUserId != null)
            {
                item.GetRoom().GetRoomUserManager().UpdateUserStatus(roomUserByUserId, false);
            }
        }
    }

    public override void OnTick(Item item)
    {
    }
}
