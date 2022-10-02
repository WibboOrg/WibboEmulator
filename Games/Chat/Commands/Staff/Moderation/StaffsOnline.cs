using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class StaffsOnline : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Output = "Les staffs en ligne: \n\n";

            List<GameClient> Staffs = WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers();

            if (Staffs.Count > 0)
            {
                foreach (GameClient Client in Staffs)
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