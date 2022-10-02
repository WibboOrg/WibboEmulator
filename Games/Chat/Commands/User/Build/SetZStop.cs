using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class SetZStop : IChatCommand
    {
        public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
        {
            userRoom.ConstruitZMode = false;
            session.SendPacket(room.GetGameMap().Model.SerializeRelativeHeightmap());

            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.setz.disabled", session.Langue));
        }
    }
}
