using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class SetSpeed : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            if (!currentRoom.CheckRights(Session, true))
            {
                return;
            }

            try
            {
                Session.GetUser().CurrentRoom.GetRoomItemHandler().SetSpeed(int.Parse(Params[1]));
            }
            catch
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));
            }

        }
    }
}
