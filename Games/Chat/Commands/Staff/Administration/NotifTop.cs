
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class NotifTop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            WibboEnvironment.GetGame().GetClientManager().SendMessage(new NotifTopComposer(Message));
        }
    }
}