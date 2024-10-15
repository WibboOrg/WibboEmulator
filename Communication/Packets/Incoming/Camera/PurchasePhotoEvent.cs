namespace WibboEmulator.Communication.Packets.Incoming.Camera;
using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class PurchasePhotoEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var photoId = Session.User.LastPhotoId;

        if (string.IsNullOrEmpty(photoId))
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.error", Session.Language));
            return;
        }

        var photoItemId = SettingsManager.GetData<int>("photo.item.id");
        if (!ItemManager.GetItem(photoItemId, out var itemData))
        {
            return;
        }

        var photoSmallItemId = SettingsManager.GetData<int>("photo.small.item.id");
        if (!ItemManager.GetItem(photoSmallItemId, out var itemDataSmall))
        {
            return;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var extraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + Session.User.Username + "\", \"s\":\"" + Session.User.Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + time + "000" + "\"}";

        using var dbClient = DatabaseManager.Connection;
        var itemSmall = ItemFactory.CreateSingleItemNullable(dbClient, itemDataSmall, Session.User, extraData);
        Session.User.InventoryComponent.TryAddItem(itemSmall);

        var item = ItemFactory.CreateSingleItemNullable(dbClient, itemData, Session.User, extraData);
        Session.User.InventoryComponent.TryAddItem(item);

        Session.SendPacket(new CameraPurchaseSuccesfullComposer());
    }
}
