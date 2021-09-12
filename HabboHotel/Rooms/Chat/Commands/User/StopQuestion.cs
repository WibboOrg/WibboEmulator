using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class StopQuestion : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
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
