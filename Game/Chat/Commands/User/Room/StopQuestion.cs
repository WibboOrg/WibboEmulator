using Wibbo.Communication.Packets.Outgoing.Rooms.Polls;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class StopQuestion : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.SendPacket(new QuestionFinishedComposer(Room.VotedNoCount, Room.VotedYesCount));

            Session.SendWhisper("Question terminée!");
        }
    }
}
