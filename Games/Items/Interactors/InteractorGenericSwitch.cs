namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

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
            var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.CanWalk = true;
            }

            item.InteractingUser = 0;
        }
        if (item.InteractingUser2 != 0)
        {
            var roomUserByUserIdTwo = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser2);
            if (roomUserByUserIdTwo != null)
            {
                roomUserByUserIdTwo.CanWalk = true;
            }

            item.InteractingUser2 = 0;
        }

        if (string.IsNullOrEmpty(item.ExtraData) && this._modes > 0)
        {
            if (item.ItemData.InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
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
            var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.CanWalk = true;
            }

            item.InteractingUser = 0;
        }
        if (item.InteractingUser2 != 0)
        {
            var roomUserByUserIdTwo = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser2);
            if (roomUserByUserIdTwo != null)
            {
                roomUserByUserIdTwo.CanWalk = true;
            }

            item.InteractingUser2 = 0;
        }
        if (string.IsNullOrEmpty(item.ExtraData) && this._modes > 0)
        {
            if (item.ItemData.InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
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
            QuestManager.ProgressUserQuest(session, QuestType.FurniSwitch, 0);
        }

        if (!userHasRights || this._modes == 0)
        {
            return;
        }

        int state;
        if (item.ItemData.InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
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

        if (session != null && session.User != null && session.User.ForceUse > -1)
        {
            newState = (session.User.ForceUse <= this._modes) ? session.User.ForceUse : 0;
        }

        if (item.ItemData.InteractionType is InteractionType.GUILD_ITEM or InteractionType.GUILD_GATE)
        {
            item.ExtraData = newState.ToString() + ";" + item.GroupId;
        }
        else
        {
            item.ExtraData = newState.ToString();
        }

        item.UpdateState();

        if (item.ItemData.AdjustableHeights.Count > 1)
        {
            if (session == null || session.User == null)
            {
                return;
            }

            if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
            {
                return;
            }

            var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
            if (roomUserByUserId != null)
            {
                item.                Room.RoomUserManager.UpdateUserStatus(roomUserByUserId, false);
            }
        }
    }

    public override void OnTick(Item item)
    {
    }
}
