using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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
