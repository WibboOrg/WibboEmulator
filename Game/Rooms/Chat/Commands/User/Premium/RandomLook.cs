using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class RandomLook : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.none || UserRoom.InGame)
            {
                return;
            }

            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Session.GetHabbo().Look = UserWardrobeDao.GetOneRandomLook(dbClient);

            if (UserRoom.transformation || UserRoom.IsSpectator)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;
            if (roomUserByHabbo == null)
            {
                return;
            }

            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
            currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));

        }
    }
}
