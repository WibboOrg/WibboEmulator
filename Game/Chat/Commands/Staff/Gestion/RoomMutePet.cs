using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class RoomMutePet : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = Session.GetUser().CurrentRoom;
            if (room == null)
            {
                return;
            }

            if (room.RoomMutePets)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.true", Session.Langue));
                room.RoomMutePets = false;
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.false", Session.Langue));
                room.RoomMutePets = true;
            }

        }
    }
}
