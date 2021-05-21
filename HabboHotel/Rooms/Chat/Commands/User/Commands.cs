using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Commands : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendHugeNotif(ButterflyEnvironment.GetGame().GetChatManager().GetCommands().GetCommandList(Session));
        }
    }
}
