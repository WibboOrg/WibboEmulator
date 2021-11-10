using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Commands : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendHugeNotif(ButterflyEnvironment.GetGame().GetChatManager().GetCommands().GetCommandList(Session));
        }
    }
}
