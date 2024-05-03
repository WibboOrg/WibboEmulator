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

    public void Parse(GameClient session, ClientPacket packet)
    {
        var photoId = session.User.LastPhotoId;

        if (string.IsNullOrEmpty(photoId))
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.error", session.Language));
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
        var extraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + session.User.Username + "\", \"s\":\"" + session.User.Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + time + "000" + "\"}";

        using var dbClient = DatabaseManager.Connection;
        var itemSmall = ItemFactory.CreateSingleItemNullable(dbClient, itemDataSmall, session.User, extraData);
        session.User.InventoryComponent.TryAddItem(itemSmall);

        var item = ItemFactory.CreateSingleItemNullable(dbClient, itemData, session.User, extraData);
        session.User.InventoryComponent.TryAddItem(item);

        session.SendPacket(new CameraPurchaseSuccesfullComposer());
    }
}
