using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RandomLook : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (UserRoom.transformation || UserRoom.IsSpectator)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Session.GetHabbo().Look = UserWardrobeDao.GetOneRandomLook(dbClient);

            Session.SendPacket(new UserChangeComposer(UserRoom, true));
            Room.SendPacket(new UserChangeComposer(UserRoom, false));

        }
    }
}
