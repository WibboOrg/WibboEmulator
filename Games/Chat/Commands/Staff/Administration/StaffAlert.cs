using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class StaffAlert : IChatCommand
    {
        public void Execute(GameClient session, Room room, RoomUser roomUser, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                return;
            }

            string messageTxt = CommandManager.MergeParams(parameters, 1);

            if (string.IsNullOrEmpty(messageTxt))
            {
                return;
            }

            foreach (GameClient Staff in WibboEnvironment.GetGame().GetGameClientManager().GetClients)
            {
                if (Staff == null)
                {
                    continue;
                }

                if (Staff.GetUser() == null)
                {
                    continue;
                }

                if (Staff.GetUser().CurrentRoom == null)
                {
                    continue;
                }

                if (Staff.GetUser().Rank < 3)
                {
                    continue;
                }

                RoomUser User = Staff.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(Staff.GetUser().Id);

                User.GetClient().SendPacket(new WhisperComposer(User.VirtualId, "[STAFF ALERT] " + messageTxt + " - " + roomUser.GetUsername(), 23));
            }
        }
    }
}
