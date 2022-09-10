using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class SetSpeed : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));
                return;
            }

            if (int.TryParse(Params[1], out int setSpeedCount))
                Room.GetRoomItemHandler().SetSpeed(setSpeedCount);
            else
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));
        }
    }
}
