using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Game.Clients;
using System.Linq;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class AllWhisper : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null)
                {
                    continue;
                }
                User.GetClient().SendPacket(new WhisperComposer(UserRoom.VirtualId, Message, 0, 0));


                /*ServerPacket MessagePack = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                MessagePack.WriteInteger(UserRoom.VirtualId);
                MessagePack.WriteString(Message);
                MessagePack.WriteInteger(0);
                MessagePack.WriteInteger(0); //Color
                MessagePack.WriteInteger(0);
                MessagePack.WriteInteger(-1);

                User.GetClient().SendPacket(MessagePack);*/
            }
        }
    }
}