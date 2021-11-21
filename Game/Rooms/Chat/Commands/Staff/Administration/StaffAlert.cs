using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class StaffAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            string MessageTxt = CommandManager.MergeParams(Params, 1);

            if (string.IsNullOrEmpty(MessageTxt))
            {
                return;
            }

            foreach (Client Staff in ButterflyEnvironment.GetGame().GetClientManager().GetClients)
            {
                if (Staff == null)
                {
                    continue;
                }

                if (Staff.GetHabbo() == null)
                {
                    continue;
                }

                if (Staff.GetHabbo().CurrentRoom == null)
                {
                    continue;
                }

                if (Staff.GetHabbo().Rank < 3)
                {
                    continue;
                }

                RoomUser User = Staff.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Staff.GetHabbo().Id);

                ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                Message.WriteInteger(User.VirtualId);
                Message.WriteString("[STAFF ALERT] " + MessageTxt + " - " + UserRoom.GetUsername());
                Message.WriteInteger(0);
                Message.WriteInteger(23);
                Message.WriteInteger(0);
                Message.WriteInteger(-1);
                User.GetClient().SendPacket(Message);
            }
        }
    }
}
