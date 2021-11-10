using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;

namespace Butterfly.Communication.RCON.Commands.User
{
    internal class AddPhotoCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parameters[1], out int Userid))
            {
                return false;
            }

            if (Userid == 0)
            {
                return false;
            }

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            string PhotoId = parameters[2];

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(4581, out ItemData ItemData))
            {
                return false;
            }

            int Time = ButterflyEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Client.GetHabbo().Username + "\", \"s\":\"" + Client.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Client.GetHabbo(), ExtraData);
            Client.GetHabbo().GetInventoryComponent().TryAddItem(Item);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserPhotoDao.Insert(dbClient, Client.GetHabbo().Id, PhotoId, Time);
            }

            Client.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Client.Langue));

            return true;
        }
    }
}
