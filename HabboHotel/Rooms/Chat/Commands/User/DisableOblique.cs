using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class DisableOblique : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            currentRoom.GetGameMap().ObliqueDisable = !currentRoom.GetGameMap().ObliqueDisable;

        }
    }
}
