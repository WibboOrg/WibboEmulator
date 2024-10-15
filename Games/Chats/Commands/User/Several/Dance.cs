namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Dance : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser user, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            Session.SendWhisper("Entre un numéro à ta danse");
            return;
        }

        if (int.TryParse(parameters[1], out var danceId))
        {
            if (danceId is > 4 or < 0)
            {
                Session.SendWhisper("Choisit un numéro entre 0 et 4");
                return;
            }

            room.SendPacket(new DanceComposer(user.VirtualId, danceId));
        }
        else
        {
            Session.SendWhisper("Entre un numéro de danse valide");
        }
    }
}