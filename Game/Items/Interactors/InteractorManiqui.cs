using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorManiqui : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
            {
                return;
            }

            if (!Item.ExtraData.Contains(";"))
            {
                return;
            }

            Rooms.Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            string[] strArray = Session.GetHabbo().Look.Split(new char[1] { '.' });
            string str1 = "";
            foreach (string str2 in strArray)
            {
                if (!str2.StartsWith("ch") && !str2.StartsWith("lg") && (!str2.StartsWith("cc") && !str2.StartsWith("ca")) && (!str2.StartsWith("sh") && !str2.StartsWith("wa")))
                {
                    str1 = str1 + str2 + ".";
                }
            }

            string str3 = str1 + Item.ExtraData.Split(new char[1] { ';' })[1];
            Session.GetHabbo().Look = str3;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateLook(dbClient, Session.GetHabbo().Id, str3);
            }

            if (room == null)
            {
                return;
            }

            RoomUser Roomuser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (Roomuser == null)
            {
                return;
            }

            if (Roomuser.transformation || Roomuser.IsSpectator)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            room.SendPacket(new UserChangeComposer(Roomuser, false));
        }
    }
}
