namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class AddPhoto : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var PhotoId = parameters[1];
        var ItemPhotoId = 4581;
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(ItemPhotoId, out var ItemData))
        {
            return;
        }

        var Time = WibboEnvironment.GetUnixTimestamp();
        var ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + session.GetUser().Username + "\", \"s\":\"" + session.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

        var Item = ItemFactory.CreateSingleItemNullable(ItemData, session.GetUser(), ExtraData);
        session.GetUser().GetInventoryComponent().TryAddItem(Item);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserPhotoDao.Insert(dbClient, session.GetUser().Id, PhotoId, Time);
        }

        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", session.Langue));
    }
}
