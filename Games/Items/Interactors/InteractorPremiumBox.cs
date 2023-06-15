namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Premium;

public class InteractorPremiumBox : FurniInteractor
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
        if (session == null || session.User == null || session.User.Premium == null || this._haveReward || !userHasRights)
        {
            return;
        }

        var room = item.GetRoom();

        if (room == null || !room.CheckRights(session, true))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);

        if (roomUser == null)
        {
            return;
        }

        this._haveReward = true;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ItemDao.DeleteById(dbClient, item.Id);

        room.RoomItemHandling.RemoveFurniture(null, item.Id);

        var premiumClubLevel = PremiumClubLevel.CLASSIC;
        var badgeCode = "WC_CLASSIC";

        if (item.GetBaseItem().InteractionType == InteractionType.PREMIUM_LEGEND)
        {
            badgeCode = "WC_LEGEND";
            premiumClubLevel = PremiumClubLevel.LEGEND;
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.PREMIUM_EPIC)
        {
            badgeCode = "WC_EPIC";
            premiumClubLevel = PremiumClubLevel.EPIC;
        }

        session.User.Premium.AddPremiumDays(dbClient, 31, premiumClubLevel);

        if (premiumClubLevel == PremiumClubLevel.LEGEND)
        {
            roomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("premiumbox.legend.convert", session.Langue)));
        }
        else if (premiumClubLevel == PremiumClubLevel.EPIC)
        {
            roomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("premiumbox.epic.convert", session.Langue)));
        }
        else
        {
            roomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("premiumbox.classic.convert", session.Langue)));
        }

        session.User.BadgeComponent.GiveBadge(badgeCode, true);

        session.User.Premium.SendPackets();
    }

    public override void OnTick(Item item)
    {
    }
}
