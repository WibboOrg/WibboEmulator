namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceTransf : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var username = Params[1];

        var roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null)
        {
            return;
        }

        var clientByUsername = roomUserByUserId.GetClient();
        if (clientByUsername == null)
        {
            return;
        }

        if (clientByUsername.GetUser().SpectatorMode)
        {
            return;
        }

        if (Params.Length is not 4 and not 3)
        {
            return;
        }

        var RoomClient = roomUserByUserId.GetClient().GetUser().CurrentRoom;
        if (RoomClient == null)
        {
            return;
        }

        var raceid = 0;
        if (Params.Length == 4)
        {
            var x = Params[3];
            if (int.TryParse(x, out var value))
            {
                int.TryParse(Params[2], out raceid);
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

        if (!roomUserByUserId.SetPetTransformation(Params[2], raceid))
        {
            session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", session.Langue));
            return;
        }

        roomUserByUserId.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.helpstop", session.Langue));

        roomUserByUserId.IsTransf = true;

        RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
        RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
    }
}
