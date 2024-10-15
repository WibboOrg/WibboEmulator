namespace WibboEmulator.Games.Items.Interactors;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Premium;

public class InteractorPremiumBox : FurniInteractor
{
    private bool _haveReward;

    public override void OnPlace(GameClient Session, Item item)
    {
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (Session == null || Session.User == null || Session.User.Premium == null || this._haveReward || !userHasRights)
        {
            return;
        }

        var room = item.Room;

        if (room == null || !room.CheckRights(Session, true))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);

        if (roomUser == null)
        {
            return;
        }

        this._haveReward = true;

        using var dbClient = DatabaseManager.Connection;
        ItemDao.DeleteById(dbClient, item.Id);

        room.RoomItemHandling.RemoveFurniture(null, item.Id);

        var premiumClubLevel = PremiumClubLevel.CLASSIC;
        var badgeCode = "WC_CLASSIC";

        if (item.ItemData.InteractionType == InteractionType.PREMIUM_LEGEND)
        {
            badgeCode = "WC_LEGEND";
            premiumClubLevel = PremiumClubLevel.LEGEND;
        }
        else if (item.ItemData.InteractionType == InteractionType.PREMIUM_EPIC)
        {
            badgeCode = "WC_EPIC";
            premiumClubLevel = PremiumClubLevel.EPIC;
        }

        Session.User.Premium.AddPremiumDays(dbClient, 31, premiumClubLevel);

        if (premiumClubLevel == PremiumClubLevel.LEGEND)
        {
            roomUser.SendWhisperChat(string.Format(LanguageManager.TryGetValue("premiumbox.legend.convert", Session.Language)));
        }
        else if (premiumClubLevel == PremiumClubLevel.EPIC)
        {
            roomUser.SendWhisperChat(string.Format(LanguageManager.TryGetValue("premiumbox.epic.convert", Session.Language)));
        }
        else
        {
            roomUser.SendWhisperChat(string.Format(LanguageManager.TryGetValue("premiumbox.classic.convert", Session.Language)));
        }

        Session.User.BadgeComponent.GiveBadge(badgeCode);

        Session.User.Premium.SendPackets();
    }

    public override void OnTick(Item item)
    {
    }
}
