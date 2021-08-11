using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TeamGameOver : IWired, IWiredEffect
    {
        private readonly int itemID;
        private Team team;
        private Room room;

        public TeamGameOver(int TeamId, int itemID, Room room)
        {
            if (TeamId < 1 || TeamId > 4)
            {
                TeamId = 1;
            }

            this.itemID = itemID;
            this.team = (Team)TeamId;
            this.room = room;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            TeamManager managerForBanzai = this.room.GetTeamManager();

            List<RoomUser> ListTeam = new List<RoomUser>();

            if (this.team == Team.blue)
            {
                ListTeam.AddRange(managerForBanzai.BlueTeam);
            }
            else if (this.team == Team.green)
            {
                ListTeam.AddRange(managerForBanzai.GreenTeam);
            }
            else if (this.team == Team.red)
            {
                ListTeam.AddRange(managerForBanzai.RedTeam);
            }
            else if (this.team == Team.yellow)
            {
                ListTeam.AddRange(managerForBanzai.YellowTeam);
            }
            else
            {
                return;
            }

            Item ExitTeleport = this.room.GetGameItemHandler().GetExitTeleport();

            foreach (RoomUser teamuser in ListTeam)
            {
                if (teamuser == null)
                {
                    continue;
                }

                managerForBanzai.OnUserLeave(teamuser);
                this.room.GetGameManager().UpdateGatesTeamCounts();
                teamuser.ApplyEffect(0);
                teamuser.Team = Team.none;

                teamuser.GetClient().SendPacket(new IsPlayingComposer(false));

                if (ExitTeleport != null)
                {
                    this.room.GetGameMap().TeleportToItem(teamuser, ExitTeleport);
                }
            }
        }

        public void Dispose()
        {
            this.room = null;
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

            Message.WriteInteger(0); //7
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
