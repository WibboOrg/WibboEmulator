namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StaffsOnline : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var output = "Les staffs en ligne: \n\n";

        var staffs = WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers();

        if (staffs.Count > 0)
        {
            foreach (var client in staffs)
            {
                if (client != null && client.GetUser() != null)
                {
                    output += $"{client.GetUser().Username} (Rank: {client.GetUser().Rank})\n";
                }
            }
        }
        else
        {
            output += "Aucun staffs en ligne!";
        }

        session.SendNotification(output);
    }
}
