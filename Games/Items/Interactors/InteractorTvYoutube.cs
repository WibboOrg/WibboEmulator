namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Televisions;
using WibboEmulator.Games.GameClients;

public class InteractorTvYoutube : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        session.SendPacket(new YoutubeTvComposer(userHasRights ? item.Id : 0, item.ExtraData));
    }

    public override void OnTick(Item item)
    {
    }
}
