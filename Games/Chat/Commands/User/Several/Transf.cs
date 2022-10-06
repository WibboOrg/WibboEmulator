namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Transf : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        if (session.GetUser().SpectatorMode || UserRoom.InGame)
        {
            return;
        }

        if (parameters.Length is 3 or 2)
        {
            var raceid = 0;
            if (parameters.Length == 3)
            {
                var x = parameters[2];
                if (int.TryParse(x, out var value))
                {
                    raceid = Convert.ToInt32(parameters[2]);
                    if (raceid is < 1 or > 50)
                    {
                        raceid = 0;
                    }
                }
            }
            else
            {
                raceid = 0;
            }

            if (!UserRoom.SetPetTransformation(parameters[1], raceid))
            {
                session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", session.Langue));
                return;
            }

            UserRoom.IsTransf = true;

            Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
            Room.SendPacket(new UsersComposer(UserRoom));
        }
        else
        {
            session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", session.Langue));
        }
    }
}
