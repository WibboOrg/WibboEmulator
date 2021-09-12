using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class IgnoreAll : IChatCommand
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
            if (Params.Length != 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                return;
            }

            clientByUsername.GetHabbo().IgnoreAll = !clientByUsername.GetHabbo().IgnoreAll;

            if (clientByUsername.GetHabbo().IgnoreAll)
            {
                UserRoom.SendWhisperChat("IgnoreAll activé");
            }
            else
            {
                UserRoom.SendWhisperChat("IgnoreAll désactivé");
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE users SET ignoreall = '" + ButterflyEnvironment.BoolToEnum(clientByUsername.GetHabbo().IgnoreAll) + "' WHERE id = " + clientByUsername.GetHabbo().Id);
            }
        }
    }
}
