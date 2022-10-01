using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class AddPhoto : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            string PhotoId = Params[1];
            int ItemPhotoId = 4581;
            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(ItemPhotoId, out ItemData ItemData))
            {
                return;
            }

            int Time = WibboEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Session.GetUser().Username + "\", \"s\":\"" + Session.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetUser(), ExtraData);
            Session.GetUser().GetInventoryComponent().TryAddItem(Item);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserPhotoDao.Insert(dbClient, Session.GetUser().Id, PhotoId, Time);
            }

            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Session.Langue));
        }
    }
}
