using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorManiqui : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Session.GetUser() == null || Item == null)
            {
                return;
            }

            if (!Item.ExtraData.Contains(";"))
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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

            if (room == null)
            {
                return;
            }

            RoomUser Roomuser = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (Roomuser == null)
            {
                return;
            }

            if (Roomuser.IsTransf || Roomuser.IsSpectator)
            {
                return;
            }

            if (!Session.GetUser().InRoom)
            {
                return;
            }

            Session.SendPacket(new FigureUpdateComposer(Session.GetUser().Look, Session.GetUser().Gender));
            room.SendPacket(new UserChangeComposer(Roomuser, false));
        }

        public override void OnTick(Item item)
        {
        }
    }
}
