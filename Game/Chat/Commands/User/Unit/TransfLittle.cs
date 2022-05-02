using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class TransfLittle : IChatCommand
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

            if (Session.GetUser().SpectatorMode || UserRoom.InGame)
            {
                return;
            }

            if (!UserRoom.SetPetTransformation("little" + Params[1], 0))
            {
                Session.SendHugeNotif(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.littleorbig.help", Session.Langue));
                return;
            }

            UserRoom.IsTransf = true;

            Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
            Room.SendPacket(new UsersComposer(UserRoom));
        }
    }
}
