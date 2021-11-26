using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class StopQuestion : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.SimplePollAnswersComposer);
            Message.WriteInteger(1);//PollId
            Message.WriteInteger(2);//Count
            Message.WriteString("0");//Négatif
            Message.WriteInteger(Room.VotedNoCount);//Nombre
            Message.WriteString("1");//Positif
            Message.WriteInteger(Room.VotedYesCount);//Nombre
            Room.SendPacket(Message);

            UserRoom.SendWhisperChat("Question terminée!");
        }
    }
}
