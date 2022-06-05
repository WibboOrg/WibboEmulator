using Wibbo.Communication.Packets.Outgoing.Rooms.Polls;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class StartQuestion : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Question = CommandManager.MergeParams(Params, 1);

            if (string.IsNullOrWhiteSpace(Question))
            {
                Session.SendWhisper("Votre question ne peut pas être vide");
                return;
            }

            Room.SendPacket(new QuestionComposer(Question));

            Room.VotedNoCount = 0;
            Room.VotedYesCount = 0;
        }
    }
}
