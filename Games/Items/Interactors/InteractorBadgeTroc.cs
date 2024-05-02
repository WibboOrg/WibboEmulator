namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;

public class InteractorBadgeTroc : FurniInteractor
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

        if (session.User.BadgeComponent.HasBadge(item.ExtraData))
        {
            //session.SendNotification("Vous posséder déjà ce badge !");
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous possèdez déjà ce badge."));
            return;
        }

        this._haveReward = true;

        using var dbClient = DatabaseManager.Connection;
        ItemDao.DeleteById(dbClient, item.Id);

        room.RoomItemHandling.RemoveFurniture(null, item.Id);

        session.User.BadgeComponent.GiveBadge(item.ExtraData);

        session.SendNotification("Vous avez reçu le badge: " + item.ExtraData + " !");
    }

    public override void OnTick(Item item)
    {
    }
}
