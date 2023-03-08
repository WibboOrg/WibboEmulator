namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Transf : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode || session.User.SpectatorMode)
        {
            return;
        }

        if (parameters.Length is 3 or 2)
        {
            var raceId = 0;
            if (parameters.Length == 3)
            {
                var x = parameters[2];
                if (int.TryParse(x, out _))
                {
                    raceId = Convert.ToInt32(parameters[2]);
                    if (raceId is < 1 or > 50)
                    {
                        raceId = 0;
                    }
                }
            }
            else
            {
                raceId = 0;
            }

            if (!userRoom.SetPetTransformation(parameters[1], raceId))
            {
                session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", session.Langue));
                return;
            }

            userRoom.IsTransf = true;

            room.SendPacket(new UserRemoveComposer(userRoom.VirtualId));
            room.SendPacket(new UsersComposer(userRoom));
        }
        else
        {
            session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", session.Langue));
        }
    }
}
