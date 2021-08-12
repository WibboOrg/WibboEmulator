using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class RoomAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room == null)
            {
                return;
            }

            string s = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(s, "<CMD>", Room.Id))
            {
                return;
            }

            ServerPacket message = new ServerPacket(ServerPacketHeader.GENERIC_ALERT);
            message.WriteString(s);
            Room.SendPacket(message);

        }
    }
}
