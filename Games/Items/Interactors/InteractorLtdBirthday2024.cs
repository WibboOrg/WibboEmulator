namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Sound;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

public class InteractorLtdBirthday2024 : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item) => item.ExtraData = "0";

    public override void OnRemove(GameClient session, Item item) => item.ExtraData = "0";

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights)
        {
            return;
        }

        item.ExtraData = item.ExtraData == "1" ? "0" : "1";
        item.UpdateState(true);

        if (item.ExtraData == "1")
        {
            item.Room?.SendPacket(new PlaySoundComposer("birthday2024", 2, true));
        }
        else
        {
            item.Room?.SendPacket(new StopSoundComposer("birthday2024"));
        }
    }

    public override void OnTick(Item item)
    {
    }
}
