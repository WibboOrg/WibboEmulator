using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;

namespace WibboEmulator.Games.Chat.Commands.Cmd
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
