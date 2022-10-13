namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

public class InteractorSwitch : FurniInteractor
{
    private readonly int _modes;

    public InteractorSwitch(int modes)
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
        if (string.IsNullOrEmpty(item.ExtraData) && this._modes > 0)
        {
            item.ExtraData = "0";
        }
    }

    public override void OnRemove(GameClient session, Item item)
    {
        if (string.IsNullOrEmpty(item.ExtraData) && this._modes > 0)
        {
            item.ExtraData = "0";
        }
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session != null)
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FURNI_SWITCH, 0);
        }

        if (this._modes == 0)
        {
            return;
        }

        RoomUser roomUser = null;
        if (session != null)
        {
            roomUser = item.GetRoom().RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
        }

        if (roomUser == null)
        {
            return;
        }

        if (!GameMap.TilesTouching(item.X, item.Y, roomUser.X, roomUser.Y))
        {
            return;
        }

        _ = int.TryParse(item.ExtraData, out var state);

        if (reverse)
        {
            item.ExtraData = (state > 0 ? state - 1 : this._modes).ToString();
        }
        else
        {
            item.ExtraData = (state < this._modes ? state + 1 : 0).ToString();
        }

        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
