namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

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

    public override void OnPlace(GameClient session, Item item)
    {
        if (string.IsNullOrEmpty(item.ExtraData) && this.Modes > 0)
        {
            item.ExtraData = "0";
        }
    }

    public override void OnRemove(GameClient session, Item item)
    {
        if (string.IsNullOrEmpty(item.ExtraData) && this.Modes > 0)
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

        if (this.Modes == 0)
        {
            return;
        }

        RoomUser roomUser = null;
        if (session != null)
        {
            roomUser = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        }

        if (roomUser == null)
        {
            return;
        }

        if (!Gamemap.TilesTouching(item.X, item.Y, roomUser.X, roomUser.Y))
        {
            return;
        }

        int.TryParse(item.ExtraData, out var state);

        if (reverse)
        {
            item.ExtraData = (state > 0 ? state - 1 : this.Modes).ToString();
        }
        else
        {
            item.ExtraData = (state < this.Modes ? state + 1 : 0).ToString();
        }

        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
