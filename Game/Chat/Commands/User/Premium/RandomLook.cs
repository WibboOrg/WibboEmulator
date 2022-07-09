using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms.Games;
using Wibbo.Game.Rooms;
using Wibbo.Communication.Packets.Outgoing.Avatar;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class RandomLook : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetUser() == null)
            {
                return;
            }

            if (UserRoom.IsTransf || UserRoom.IsSpectator)
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                Session.GetUser().Look = UserWardrobeDao.GetOneRandomLook(dbClient);

            Session.SendPacket(new FigureUpdateComposer(Session.GetUser().Look, Session.GetUser().Gender));
            Session.SendPacket(new UserChangeComposer(UserRoom, true));
            Room.SendPacket(new UserChangeComposer(UserRoom, false));

        }
    }
}
