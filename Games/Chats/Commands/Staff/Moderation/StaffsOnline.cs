namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class StaffsOnline : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var output = "Les Staffs en ligne: \n\n";

        var staffs = GameClientManager.StaffUsers;

        if (staffs.Count > 0)
        {
            foreach (var client in staffs)
            {
                if (client != null && client.User != null)
                {
                    output += $"{client.User.Username} (Rank: {client.User.Rank})\n";
                }
            }
        }
        else
        {
            output += "Aucun Staff n'est disponible pour le moment";
        }

        session.SendNotification(output);
    }
}
