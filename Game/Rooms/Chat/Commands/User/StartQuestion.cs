using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class StartQuestion : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Question = CommandManager.MergeParams(Params, 1);

            if (string.IsNullOrWhiteSpace(Question))
            {
                UserRoom.SendWhisperChat("Votre question ne peu pas être vide");
                return;
            }

            ServerPacket MessageTwo = new ServerPacket(ServerPacketHeader.QuestionInfoComposer);
            MessageTwo.WriteString("MATCHING_POLL"); //Type
            MessageTwo.WriteInteger(1);//pollId
            MessageTwo.WriteInteger(1);//questionId
            MessageTwo.WriteInteger(60);//Duration
            MessageTwo.WriteInteger(1); //id
            MessageTwo.WriteInteger(1);//number
            MessageTwo.WriteInteger(3);//type (1 ou 2)
            MessageTwo.WriteString(Question);//content
            MessageTwo.WriteInteger(0);
            MessageTwo.WriteInteger(0);
            //MessageTwo.WriteString("0");
            //MessageTwo.WriteString("1");
            Room.SendPacket(MessageTwo);

            Room.VotedNoCount = 0;
            Room.VotedYesCount = 0;
        }
    }
}
