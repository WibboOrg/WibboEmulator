using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class DND : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().IgnoreRoomInvites = !Session.GetUser().IgnoreRoomInvites;
            Session.SendWhisper("Tu " + (Session.GetUser().IgnoreRoomInvites == true ? "acceptes" : "refuses") + " les messages dans ta console d'amis");
        }
    }
}
