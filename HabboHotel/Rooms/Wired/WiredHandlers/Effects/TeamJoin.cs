using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TeamJoin : IWired, IWiredEffect
    {
        private readonly int itemID;
        private Team team;

        public TeamJoin(int TeamId, int itemID)
        {
            if (TeamId < 1 || TeamId > 4)
            {
                TeamId = 1;
            }

            this.itemID = itemID;
            this.team = (Team)TeamId;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user != null && !user.IsBot && user.GetClient() != null && user.Room != null)
            {

                TeamManager managerForFreeze = user.Room.GetTeamManager();

                if (user.Team != Team.none)
                {
                    managerForFreeze.OnUserLeave(user);
                }

                user.Team = this.team;
                managerForFreeze.AddUser(user);
                user.Room.GetGameManager().UpdateGatesTeamCounts();

                int EffectId = ((int)this.team + 39);
                user.ApplyEffect(EffectId);

                user.GetClient().SendPacket(new IsPlayingComposer(true));
            }
        }

        public void Dispose()
        {

        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, ((int)this.team).ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
            {
                return;
            }

            bool result = int.TryParse(row[0].ToString(), out int number);
            if (result)
            {
                this.team = (Team)number;
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(1);
            Message.WriteInteger((int)this.team);
            Message.WriteInteger(0);
            Message.WriteInteger(9);
            Message.WriteInteger(0);
            Message.WriteInteger(0);

            Session.SendPacket(Message);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
