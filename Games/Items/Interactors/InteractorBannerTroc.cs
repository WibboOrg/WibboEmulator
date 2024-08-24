namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;

public class InteractorBannerTroc : FurniInteractor
{
    private bool _haveReward;

    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || this._haveReward || !userHasRights)
        {
            return;
        }

        var room = item.Room;

        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        if (!int.TryParse(item.ExtraData, out var bannerId))
        {
            return;
        }

        if (!BannerManager.TryGetBannerById(bannerId, out var banner) || session.User.BannerComponent.BannerList.Contains(banner))
        {
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous possèdez déjà cette bannière."));
            return;
        }

        this._haveReward = true;

        using var dbClient = DatabaseManager.Connection;
        ItemDao.DeleteById(dbClient, item.Id);

        room.RoomItemHandling.RemoveFurniture(null, item.Id);

        session.User.BannerComponent.AddBanner(dbClient, bannerId);

        session.SendNotification("Vous avez reçu la bannière : " + bannerId + " !");
    }

    public override void OnTick(Item item)
    {
    }
}
