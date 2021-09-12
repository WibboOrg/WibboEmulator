using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class UnloadRoom : IChatCommand
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
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(int.Parse(Params[1]));
            if (room == null)
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        }
    }
}
