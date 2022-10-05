namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

internal class Transf : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        if (session.GetUser().SpectatorMode || UserRoom.InGame)
        {
            return;
        }

        if (Params.Length is 3 or 2)
        {
            var raceid = 0;
            if (Params.Length == 3)
            {
                var x = Params[2];
                if (int.TryParse(x, out var value))
                {
                    raceid = Convert.ToInt32(Params[2]);
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

            if (!UserRoom.SetPetTransformation(Params[1], raceid))
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
