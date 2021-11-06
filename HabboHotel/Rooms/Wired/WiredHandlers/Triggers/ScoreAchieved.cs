using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class ScoreAchieved : IWired
    {
        private Item item;
        private WiredHandler handler;
        private int scoreLevel;
        //private bool used;
        private readonly TeamScoreChangedDelegate scoreChangedDelegate;
        //private RoomEventDelegate gameEndDeletgate;

        public ScoreAchieved(Item item, WiredHandler handler, int scoreLevel, GameManager gameManager)
        {
            this.item = item;
            this.handler = handler;
            this.scoreLevel = scoreLevel;
            //this.used = false;
            this.scoreChangedDelegate = new TeamScoreChangedDelegate(this.gameManager_OnScoreChanged);
            //this.gameEndDeletgate = new RoomEventDelegate(this.gameManager_OnGameEnd);
            gameManager.OnScoreChanged += this.scoreChangedDelegate;
            //gameManager.OnGameEnd += this.gameEndDeletgate;
        }

        //private void gameManager_OnGameEnd(object sender, EventArgs e)
        //{
        //this.used = false;
        //}

        private void gameManager_OnScoreChanged(object sender, TeamScoreChangedArgs e)
        {
            if (e.Points <= (this.scoreLevel - 1))// || this.used)
            {
                return;
            }
            //this.used = true;
            this.handler.ExecutePile(this.item.Coordinate, e.user, null);
        }

        public void Dispose()
        {
            this.handler.GetRoom().GetGameManager().OnScoreChanged -= this.scoreChangedDelegate;
            //this.handler.GetRoom().GetGameManager().OnGameEnd -= this.gameEndDeletgate;
            this.item = null;
            this.handler = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.scoreLevel.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if(int.TryParse(row["trigger_data"].ToString(), out int score))
                this.scoreLevel = score;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(1);
            Message.WriteInteger(this.scoreLevel);
            Message.WriteInteger(0);
            Message.WriteInteger(10);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
