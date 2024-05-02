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
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
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
        var extraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + session.User.Username + "\", \"s\":\"" + session.User.Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + time + "000" + "\"}";

        using var dbClient = DatabaseManager.Connection;

        var item = ItemFactory.CreateSingleItemNullable(dbClient, itemData, session.User, extraData);
        session.User.InventoryComponent.TryAddItem(item);

        UserPhotoDao.Insert(dbClient, session.User.Id, photoId, time);

        session.SendNotification(LanguageManager.TryGetValue("notif.buyphoto.valide", session.Language));
    }
}
