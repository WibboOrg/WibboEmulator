using Butterfly.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class StaffsOnline : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Output = "Les staffs en ligne\n";

            List<GameClient> Staffs = ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers();

            if (Staffs.Count > 0)
            {
                foreach (GameClient Client in Staffs)
                {
                    if (Client != null && Client.GetHabbo() != null)
                        Output += Client.GetHabbo().Username + "\n";
                }
            }
            else
            {
                Output += "Aucun staffs en ligne!";
            }

            Session.SendNotification(Output);
        }
    }
}