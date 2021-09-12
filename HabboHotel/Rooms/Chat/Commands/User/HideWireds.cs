using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class HideWireds : IChatCommand
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
            if (currentRoom == null)
            {
                return;
            }

            currentRoom.RoomData.HideWireds = !currentRoom.RoomData.HideWireds;

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE rooms SET allow_hidewireds = '" + (currentRoom.RoomData.HideWireds ? 1 : 0) + "' WHERE id = " + currentRoom.Id);
            }

            if (currentRoom.RoomData.HideWireds)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", Session.Langue));
            }
        }
    }
}
