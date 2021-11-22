using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class CollisionTeam : IWiredEffect, IWired
    {
        private Room room;
        private WiredHandler handler;
        private readonly int itemID;
        private Team team;
        private bool isDisposed;

        public CollisionTeam(int TeamId, Room room, WiredHandler handler, int itemID)
        {
            if (TeamId < 1 || TeamId > 4)
            {
                TeamId = 1;
            }

            this.team = (Team)TeamId;
            this.room = room;
            this.handler = handler;
            this.itemID = itemID;
            this.isDisposed = false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            this.HandleItems();
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.room = null;
            this.handler = null;
        }

        private void HandleItems()
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

            if (ListTeam.Count == 0)
            {
                return;
            }

            foreach (RoomUser teamUser in ListTeam)
            {
                if (teamUser == null)
                {
                    continue;
                }

                this.handler.TriggerCollision(teamUser, null);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, ((int)this.team).ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int number))
                this.team = (Team)number;
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

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
