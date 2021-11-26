using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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
            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(ItemPhotoId, out ItemData ItemData))
            {
                return;
            }

            int Time = ButterflyEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Session.GetHabbo().Username + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserPhotoDao.Insert(dbClient, Session.GetHabbo().Id, PhotoId, Time);
            }

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Session.Langue));
        }
    }
}
