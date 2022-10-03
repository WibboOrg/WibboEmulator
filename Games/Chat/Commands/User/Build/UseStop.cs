using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class UseStop : IChatCommand
    {
        public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
        {
            session.GetUser().ForceUse = -1;

            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.use.disabled", session.Langue));
        }
    }
}