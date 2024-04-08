namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;

public class InteractorBannerGift : FurniInteractor
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

        var room = item.GetRoom();

        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(1000009301, out var bannerTrocData))
        {
            return;
        }

        var banner = WibboEnvironment.GetGame().GetBannerManager().GetOneRandomBanner();

        if (banner == null)
        {
            return;
        }

        this._haveReward = true;

        room.RoomItemHandling.RemoveFurniture(session, item.Id);

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        ItemDao.UpdateBaseItemAndExtraData(dbClient, item.Id, bannerTrocData.Id, banner.Id.ToString());

        item.BaseItem = bannerTrocData.Id;
        item.ResetBaseItem(room);
        item.ExtraData = banner.Id.ToString();

        var itemIsInRoom = true;
        if (!room.RoomItemHandling.SetFloorItem(session, item, item.X, item.Y, item.Rotation, true, false, true))
        {
            ItemDao.UpdateResetRoomId(dbClient, item.Id);

            session.User.InventoryComponent.TryAddItem(item);

            itemIsInRoom = false;
        }

        session.SendPacket(new OpenGiftComposer(item.Data, item.ExtraData, item, itemIsInRoom));
    }

    public override void OnTick(Item item)
    {
    }
}
