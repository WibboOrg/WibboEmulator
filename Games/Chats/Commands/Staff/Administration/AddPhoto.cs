namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class AddPhoto : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var photoId = parameters[1];
        var itemPhotoId = SettingsManager.GetData<int>("photo.item.id");
        if (!ItemManager.GetItem(itemPhotoId, out var itemData))
        {
            return;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var extraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + Session.User.Username + "\", \"s\":\"" + Session.User.Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + time + "000" + "\"}";

        using var dbClient = DatabaseManager.Connection;

        var item = ItemFactory.CreateSingleItemNullable(dbClient, itemData, Session.User, extraData);
        Session.User.InventoryComponent.TryAddItem(item);

        UserPhotoDao.Insert(dbClient, Session.User.Id, photoId, time);

        Session.SendNotification(LanguageManager.TryGetValue("notif.buyphoto.valide", Session.Language));
    }
}
