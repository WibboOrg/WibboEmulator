using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class StaffsOnline : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Output = "Les staffs en ligne: \n\n";

            List<Client> Staffs = WibboEnvironment.GetGame().GetClientManager().GetStaffUsers();

            if (Staffs.Count > 0)
            {
                foreach (Client Client in Staffs)
                {
                    if (Client != null && Client.GetUser() != null)
                        Output += $"{Client.GetUser().Username} (Rank: {Client.GetUser().Rank})\n";
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