namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.Items;

internal class AddPhotoCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var Userid))
        {
            return false;
        }

        if (Userid == 0)
        {
            return false;
        }

        var Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Userid);
        if (Client == null)
        {
            return false;
        }

        var PhotoId = parameters[2];

        var photoItemId = WibboEnvironment.GetSettings().GetData<int>("photo.item.id");
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(photoItemId, out var ItemData))
        {
            return false;
        }

        var Time = WibboEnvironment.GetUnixTimestamp();
        var ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Client.GetUser().Username + "\", \"s\":\"" + Client.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

        var Item = ItemFactory.CreateSingleItemNullable(ItemData, Client.GetUser(), ExtraData);
        Client.GetUser().GetInventoryComponent().TryAddItem(Item);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserPhotoDao.Insert(dbClient, Client.GetUser().Id, PhotoId, Time);
        }

        Client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Client.Langue));

        return true;
    }
}
