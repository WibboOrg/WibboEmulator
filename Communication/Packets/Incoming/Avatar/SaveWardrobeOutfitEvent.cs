using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int slotId = Packet.PopInt();
            string look = Packet.PopString();
            string gender = Packet.PopString();

            if (slotId < 1 || slotId > 10)
            {
                return;
            }

            if (gender != "M" && gender != "F")
            {
                return;
            }

            look = ButterflyEnvironment.GetFigureManager().ProcessFigure(look, gender, true);

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT null FROM user_wardrobe WHERE user_id = '" + Session.GetHabbo().Id + "' AND slot_id = '" + slotId + "';");
                queryreactor.AddParameter("look", look);
                queryreactor.AddParameter("gender", gender.ToUpper());
                if (queryreactor.GetRow() != null)
                {
                    queryreactor.SetQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = " + Session.GetHabbo().Id + " AND slot_id = " + slotId + ";");
                    queryreactor.AddParameter("look", look);
                    queryreactor.AddParameter("gender", gender.ToUpper());
                    queryreactor.RunQuery();
                }
                else
                {
                    queryreactor.SetQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (" + Session.GetHabbo().Id + "," + slotId + ",@look,@gender)");
                    queryreactor.AddParameter("look", look);
                    queryreactor.AddParameter("gender", gender.ToUpper());
                    queryreactor.RunQuery();
                }
            }
        }
    }
}
