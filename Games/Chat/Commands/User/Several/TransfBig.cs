using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class TransfBig : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetUser().SpectatorMode)
            {
                return;
            }

            if (!UserRoom.SetPetTransformation("big" + Params[1], 0))
            {
                Session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.littleorbig.help", Session.Langue));
                return;
            }

            UserRoom.IsTransf = true;

            Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
            Room.SendPacket(new UsersComposer(UserRoom));
        }
    }
}
