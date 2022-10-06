namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Items;

internal class AddPhotoCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userId))
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (client == null)
        {
            return false;
        }

        var photoId = parameters[2];

        var photoItemId = WibboEnvironment.GetSettings().GetData<int>("photo.item.id");
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(photoItemId, out var itemData))
        {
            return false;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var extraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + client.GetUser().Username + "\", \"s\":\"" + client.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + time + "000" + "\"}";

        var item = ItemFactory.CreateSingleItemNullable(itemData, client.GetUser(), extraData);
        _ = client.GetUser().GetInventoryComponent().TryAddItem(item);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserPhotoDao.Insert(dbClient, client.GetUser().Id, photoId, time);
        }

        client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", client.Langue));

        return true;
    }
}
