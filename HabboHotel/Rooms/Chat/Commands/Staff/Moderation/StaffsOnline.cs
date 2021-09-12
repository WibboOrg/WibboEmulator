using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class StaffsOnline : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Output = "Les staffs en ligne\n";

            using (IQueryAdapter dbclient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                //dbclient.SetQuery("SELECT username FROM users WHERE online ='1'");
                dbclient.SetQuery("SELECT username FROM users WHERE online = '1' AND rank > '2'");
                dbclient.RunQuery();

                DataTable Table = dbclient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        Output += Row["username"].ToString() + "\n";
                    }
                }
                else
                {
                    Output += "Aucun staffs en ligne!";
                }
            }
            Session.SendNotification(Output);
        }
    }
}