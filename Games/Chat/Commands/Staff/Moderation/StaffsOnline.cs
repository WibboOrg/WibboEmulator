namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StaffsOnline : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var Output = "Les staffs en ligne: \n\n";

        var Staffs = WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers();

        if (Staffs.Count > 0)
        {
            foreach (var Client in Staffs)
            {
                if (Client != null && Client.GetUser() != null)
                {
                    Output += $"{Client.GetUser().Username} (Rank: {Client.GetUser().Rank})\n";
                }
            }
        }
        else
        {
            Output += "Aucun staffs en ligne!";
        }

        session.SendNotification(Output);
    }
}