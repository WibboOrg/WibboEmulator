namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class AddPhoto : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var photoId = parameters[1];
        var itemPhotoId = WibboEnvironment.GetSettings().GetData<int>("photo.item.id");
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(itemPhotoId, out var itemData))
        {
            return;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var extraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + session.GetUser().Username + "\", \"s\":\"" + session.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + time + "000" + "\"}";

        var item = ItemFactory.CreateSingleItemNullable(itemData, session.GetUser(), extraData);
        _ = session.GetUser().InventoryComponent.TryAddItem(item);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserPhotoDao.Insert(dbClient, session.GetUser().Id, photoId, time);
        }

        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", session.Langue));
    }
}
