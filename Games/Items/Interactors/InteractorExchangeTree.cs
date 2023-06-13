namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

public class InteractorExchangeTree : FurniInteractor
{
    private bool _haveReward;

    public override void OnPlace(GameClient session, Item item) => item.ReqUpdate(60);

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

        var roomUser = item.GetRoom().RoomUserManager.GetRoomUserByUserId(session.User.Id);

        if (roomUser == null)
        {
            return;
        }

        var days = 16;
        switch (item.Data.InteractionType)
        {
            case InteractionType.EXCHANGE_TREE:
                days = 2;
                break;
            case InteractionType.EXCHANGE_TREE_CLASSIC:
                days = 4;
                break;
            case InteractionType.EXCHANGE_TREE_EPIC:
                days = 8;
                break;
            case InteractionType.EXCHANGE_TREE_LEGEND:
                days = 16;
                break;
        }

        var expireSeconds = days * 24 * 60 * 60;

        _ = int.TryParse(item.ExtraData, out var activateTime);

        var timeLeft = DateTime.UnixEpoch.AddSeconds(activateTime + expireSeconds) - DateTimeOffset.UtcNow;

        if (timeLeft.TotalSeconds > 0)
        {
            roomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("tree.exchange.timeout", session.Langue), timeLeft.Days, timeLeft.Hours, timeLeft.Minutes));
        }
        else if (timeLeft.TotalSeconds <= 0)
        {
            this._haveReward = true;

            var rewards = 0;
            switch (item.Data.InteractionType)
            {
                case InteractionType.EXCHANGE_TREE:
                    rewards = WibboEnvironment.GetRandomNumber(100, 105);
                    break;
                case InteractionType.EXCHANGE_TREE_CLASSIC:
                    rewards = WibboEnvironment.GetRandomNumber(500, 550);
                    break;
                case InteractionType.EXCHANGE_TREE_EPIC:
                    rewards = WibboEnvironment.GetRandomNumber(1000, 1150);
                    break;
                case InteractionType.EXCHANGE_TREE_LEGEND:
                    rewards = WibboEnvironment.GetRandomNumber(2000, 2400);
                    break;
            }

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            ItemDao.DeleteById(dbClient, item.Id);

            room.RoomItemHandling.RemoveFurniture(null, item.Id);

            session.User.WibboPoints += rewards;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

            UserDao.UpdateAddPoints(dbClient, session.User.Id, rewards);

            roomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("tree.exchange.convert", session.Langue), rewards));
        }
    }

    public override void OnTick(Item item)
    {
        item.UpdateCounter = 60;
        item.UpdateState(false);
    }
}
