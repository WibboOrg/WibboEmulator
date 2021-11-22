using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
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

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int number))
            {
                this.team = (Team)number;
            }
        }

        public void OnTrigger(Client Session, int SpriteId)
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
    }
}
