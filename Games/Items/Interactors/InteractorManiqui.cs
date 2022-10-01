using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorManiqui : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Session.GetUser() == null || Item == null)
            {
                return;
            }

            if (!Item.ExtraData.Contains(';'))
            {
                return;
            }

            string[] lookSplit = Session.GetUser().Look.Split(new char[1] { '.' });
            string lookCode = "";
            foreach (string part in lookSplit)
            {
                if (!part.StartsWith("ch") && !part.StartsWith("lg") && (!part.StartsWith("cc") && !part.StartsWith("ca")) && (!part.StartsWith("sh") && !part.StartsWith("wa")))
                {
                    lookCode = lookCode + part + ".";
                }
            }

            string look = lookCode + Item.ExtraData.Split(new char[1] { ';' })[1];
            Session.GetUser().Look = look;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateLook(dbClient, Session.GetUser().Id, look);
            }


            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUser == null)
            {
                return;
            }

            if (roomUser.IsTransf || roomUser.IsSpectator)
            {
                return;
            }

            if (!Session.GetUser().InRoom)
            {
                return;
            }

            Session.SendPacket(new FigureUpdateComposer(Session.GetUser().Look, Session.GetUser().Gender));
            room.SendPacket(new UserChangeComposer(roomUser, false));
        }

        public override void OnTick(Item item)
        {
        }
    }
}
