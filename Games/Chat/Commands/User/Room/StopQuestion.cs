using WibboEmulator.Communication.Packets.Outgoing.Rooms.Polls;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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
