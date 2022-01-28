using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Polls;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class StopQuestion : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.SendPacket(new QuestionFinishedComposer(Room.VotedNoCount, Room.VotedYesCount));

            UserRoom.SendWhisperChat("Question terminée!");
        }
    }
}
