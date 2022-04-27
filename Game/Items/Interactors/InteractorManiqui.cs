using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
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

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            string[] strArray = Session.GetUser().Look.Split(new char[1] { '.' });
            string str1 = "";
            foreach (string str2 in strArray)
            {
                if (!str2.StartsWith("ch") && !str2.StartsWith("lg") && (!str2.StartsWith("cc") && !str2.StartsWith("ca")) && (!str2.StartsWith("sh") && !str2.StartsWith("wa")))
                {
                    str1 = str1 + str2 + ".";
                }
            }

            string str3 = str1 + Item.ExtraData.Split(new char[1] { ';' })[1];
            Session.GetUser().Look = str3;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateLook(dbClient, Session.GetUser().Id, str3);
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

            if (Roomuser.transformation || Roomuser.IsSpectator)
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
