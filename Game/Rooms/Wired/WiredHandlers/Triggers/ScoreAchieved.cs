using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class ScoreAchieved : IWired
    {
        private Item item;
        private WiredHandler handler;
        private int scoreLevel;
        private readonly TeamScoreChangedDelegate scoreChangedDelegate;

        public ScoreAchieved(Item item, WiredHandler handler, int scoreLevel, GameManager gameManager)
        {
            this.item = item;
            this.handler = handler;
            this.scoreLevel = scoreLevel;
            this.scoreChangedDelegate = new TeamScoreChangedDelegate(this.gameManager_OnScoreChanged);
            gameManager.OnScoreChanged += this.scoreChangedDelegate;
        }

        private void gameManager_OnScoreChanged(object sender, TeamScoreChangedArgs e)
        {
            if (e.Points <= (this.scoreLevel - 1))
            {
                return;
            }

            this.handler.ExecutePile(this.item.Coordinate, e.user, null);
        }

        public void Dispose()
        {
            this.handler.GetRoom().GetGameManager().OnScoreChanged -= this.scoreChangedDelegate;
            this.item = null;
            this.handler = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.scoreLevel.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int score))
                this.scoreLevel = score;
        }

        public void OnTrigger(Client Session, int SpriteId)
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
