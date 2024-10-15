namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Televisions;
using WibboEmulator.Games.GameClients;

public class InteractorTvYoutube : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item)
    {
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        Session.SendPacket(new YoutubeTvComposer(userHasRights ? item.Id : 0, item.ExtraData));
    }

    public override void OnTick(Item item)
    {
    }
}
