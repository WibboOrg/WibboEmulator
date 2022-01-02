using Butterfly.Game.Clients;
using System.Collections.Generic;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class StaffsOnline : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Output = "Les staffs en ligne\n";

            List<Client> Staffs = ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers();

            if (Staffs.Count > 0)
            {
                foreach (Client Client in Staffs)
                {
                    if (Client != null && Client.GetHabbo() != null)
                        Output += $"{Client.GetHabbo().Username} (Rank: {Client.GetHabbo().Rank})\n";
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